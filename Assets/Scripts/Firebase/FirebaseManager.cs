using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FirebaseREST;
using SimpleJSON;
using UnityEngine;

namespace Firebase {
    public class FirebaseManager : MonoBehaviour {

        public static FirebaseManager Instance { get; private set; }

        private FirebaseAuth _auth;
        private FirebaseDatabase _database;

        private string _userId = String.Empty;
        
        private const int DefaultTimeout = 10;
        
        private readonly string FMT = "O";
        
        private void OnEnable() {
            if (Instance != null) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            _auth = FirebaseAuth.Instance;
            _database = FirebaseDatabase.Instance;

            Authenticate();
        }
        
        private string DateTimeToString(DateTime d) => d.ToString(FMT);

        private DateTime StringToDateTime(string s) => DateTime.ParseExact(s, FMT, CultureInfo.InvariantCulture);

        public void Authenticate(Action<bool> onComplete = null) {
            var savedRefreshToken = PlayerPrefs.GetString(SavedKeys.RefreshToken);
            var savedIdToken = PlayerPrefs.GetString(SavedKeys.IdToken);
            var savedExpiresIn = PlayerPrefs.GetString(SavedKeys.ExpiresIn);
            var savedRefreshedAt = PlayerPrefs.GetString(SavedKeys.RefreshedAt);
            var savedUserId = PlayerPrefs.GetString(SavedKeys.UserId);
            
            if (!string.IsNullOrWhiteSpace(savedRefreshToken) && !string.IsNullOrWhiteSpace(savedIdToken) &&
                !string.IsNullOrWhiteSpace(savedExpiresIn) && !string.IsNullOrWhiteSpace(savedRefreshedAt) &&
                !string.IsNullOrWhiteSpace(savedUserId)) {

                _auth.TokenData = new TokenData(savedIdToken, savedRefreshToken, savedExpiresIn, StringToDateTime(savedRefreshedAt));
                _userId = savedUserId;
                
                Debug.Log($"Session restored [{_userId}]");
                
            } else {
                _auth.SignInAnonymously(10, res => {
                    if (res.success) {
                        PlayerPrefs.SetString(SavedKeys.RefreshToken, res.data.RefreshToken);
                        PlayerPrefs.SetString(SavedKeys.IdToken, res.data.IdToken);
                        PlayerPrefs.SetString(SavedKeys.ExpiresIn, res.data.ExpiresIn);
                        PlayerPrefs.SetString(SavedKeys.RefreshedAt, DateTimeToString(DateTime.Now));
                        
                        _auth.FetchUserInfo(DefaultTimeout, data => {
                            if (data.success) {
                                _userId = data.data.FirstOrDefault()?.localId;
                                if (!string.IsNullOrWhiteSpace(_userId))
                                    PlayerPrefs.SetString(SavedKeys.UserId, _userId);
                                PlayerPrefs.Save();
                                
                                Debug.Log($"Successfully logged-in [{_userId}]");
                                onComplete?.Invoke(true);
                            } else {
                                Debug.LogError("Authentication failure : failed to fetch UserInfo");
                                onComplete?.Invoke(false);
                            }
                        });
                    } else {
                        Debug.LogError("Authentication failure: failed to SignIn");
                        onComplete?.Invoke(false);
                    }
                });
            }
        }

        public void AddScoreEntry(ScoreEntry entry, Action onSuccess, Action onError) {
            var userScoresRef = _database.GetReference("user-scores/" + _userId);
            userScoresRef.Push(null, 10, push => {
                if (push.success){
                    var scoreEntryRef = _database.GetReference("user-scores/" + _userId + "/" + push.data);
                    scoreEntryRef.SetRawJsonValueAsync(JsonUtility.ToJson(entry), DefaultTimeout,set => {
                        if (set.success) {
                            onSuccess?.Invoke();
                        } else {
                            Debug.LogError("Write failed : " + set.message);
                            onError?.Invoke();
                        }
                    });
                } else {
                    Debug.LogError("Push failed : " + push.message);
                    onError?.Invoke();
                }
            });
        }
        
        public void GetLeaderboard(Action<List<ScoreEntry>> onSuccess, Action onError, uint limit = 0) {
            
            var reference = _database.GetReference("user-scores");
            reference.GetValueAsync(DefaultTimeout, res => {
                if (res.success) {
                    var scores = new List<ScoreEntry>();
                    
                    var users = JSON.Parse(res.data.GetRawJsonValue());
                    foreach (var userscore in users.Children) {
                        foreach (var entry in userscore.Children) {
                            scores.Add(new ScoreEntry(entry["username"], entry["score"]));
                        }
                    }

                    var sortedLeaderboard = scores.OrderByDescending(s => s.score).ToList();
                    onSuccess?.Invoke(limit > 0 ? sortedLeaderboard.Take((int)limit).ToList() : sortedLeaderboard);
                    
                } else {
                    Debug.LogError("Get Leaderboard failed : " + res.message);
                    onError?.Invoke();
                }
            });
        }
        
        public void ClearCache() {
            PlayerPrefs.SetString(SavedKeys.SavedUsername, string.Empty);
            PlayerPrefs.SetString(SavedKeys.RefreshToken, string.Empty);
            PlayerPrefs.SetString(SavedKeys.IdToken, string.Empty);
            PlayerPrefs.SetString(SavedKeys.ExpiresIn, string.Empty);
            PlayerPrefs.SetString(SavedKeys.RefreshedAt, string.Empty);
            PlayerPrefs.SetString(SavedKeys.UserId, string.Empty);
            PlayerPrefs.Save();
        }

        public string GetUserId() => _userId;

        public bool IsSignedIn() => _auth.IsSignedIn;
    }
}

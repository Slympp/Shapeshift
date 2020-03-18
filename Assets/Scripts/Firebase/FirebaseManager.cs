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
        
        private const string REFRESH_TOKEN = "REFRESH_TOKEN";
        private const string ID_TOKEN = "ID_TOKEN";
        private const string EXPIRES_IN = "EXPIRES_IN";
        private const string REFRESHED_AT = "REFRESHED_AT";
        private const string USER_ID = "USER_ID";
        private const int DEFAULT_TIMEOUT = 10;
        
        private const string FMT = "O";

        void Awake() {
            
            if (Instance != null) {
                Destroy(gameObject);
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        void Start() {
           _auth = FirebaseAuth.Instance;
           _database = FirebaseDatabase.Instance;

           Authenticate();
        }

        private string DateTimeToString(DateTime d) {
            var v = d.ToString(FMT);
            Debug.Log($"{d} => {v}");
            return v;
        }

        private DateTime StringToDateTime(string s) {
            var v = DateTime.ParseExact(s, FMT, CultureInfo.InvariantCulture);
            Debug.Log($"{s} => {v}");
            return v;
        }

        void Authenticate() {
            var savedRefreshToken = PlayerPrefs.GetString(REFRESH_TOKEN);
            var savedIdToken = PlayerPrefs.GetString(ID_TOKEN);
            var savedExpiresIn = PlayerPrefs.GetString(EXPIRES_IN);
            var savedRefreshedAt = PlayerPrefs.GetString(REFRESHED_AT);
            var savedUserId = PlayerPrefs.GetString(USER_ID);
            
            if (!string.IsNullOrWhiteSpace(savedRefreshToken) && !string.IsNullOrWhiteSpace(savedIdToken) &&
                !string.IsNullOrWhiteSpace(savedExpiresIn) && !string.IsNullOrWhiteSpace(savedRefreshedAt) &&
                !string.IsNullOrWhiteSpace(savedUserId)) {

                _auth.TokenData = new TokenData(savedIdToken, savedRefreshToken, savedExpiresIn, StringToDateTime(savedRefreshedAt));
                _userId = savedUserId;
                
                Debug.Log($"Session restored [{_userId}]");
                
            } else {
                _auth.SignInAnonymously(10, res => {
                    if (res.success) {
                        PlayerPrefs.SetString(REFRESH_TOKEN, res.data.RefreshToken);
                        PlayerPrefs.SetString(ID_TOKEN, res.data.IdToken);
                        PlayerPrefs.SetString(EXPIRES_IN, res.data.ExpiresIn);
                        PlayerPrefs.SetString(REFRESHED_AT, DateTimeToString(DateTime.Now));
                        
                        _auth.FetchUserInfo(DEFAULT_TIMEOUT, data => {
                            if (data.success) {
                                _userId = data.data.FirstOrDefault()?.localId;
                                if (!string.IsNullOrWhiteSpace(_userId))
                                    PlayerPrefs.SetString(USER_ID, _userId);
                                
                                PlayerPrefs.Save();
                                Debug.Log($"Successfully logged-in [{_userId}]");
                            } else
                                Debug.LogError("Authentication failure : failed to fetch UserInfo");
                        });
                    } else
                        Debug.LogError("Authentication failure: failed to SignIn");
                });
            }
        }

        public void AddScoreEntry(ScoreEntry entry) {

            if (!_auth.IsSignedIn) {
                Debug.LogError("AddScoreEntry failed : user is not signed in");
                return;
            }
            
            var userScoresRef = _database.GetReference("user-scores/" + _userId);
            userScoresRef.Push(null, 10, push => {
                if (push.success){
                    var scoreEntryRef = _database.GetReference("user-scores/" + _userId + "/" + push.data);
                    scoreEntryRef.SetRawJsonValueAsync(JsonUtility.ToJson(entry), DEFAULT_TIMEOUT,set => {
                        if (set.success) {
                            Debug.Log("Write success");
                        } else {
                            Debug.Log("Write failed : " + set.message);
                        }
                    });
                } else
                    Debug.LogError("Push failed : " + push.message);
            });
        }
        
        public void GetLeaderboard(Action<List<ScoreEntry>> onSuccess, uint limit = 0) {
            
            var reference = _database.GetReference("user-scores");
            reference.GetValueAsync(DEFAULT_TIMEOUT, res => {
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
                    
                } else
                    Debug.Log("Get Leaderboard failed : " + res.message);
            });
        }
    }
}

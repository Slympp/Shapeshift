using System.Collections.Generic;
using Firebase;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MainMenu {
    public class MainMenuManager : MonoBehaviour {

        private FirebaseManager _manager;

        [SerializeField] private TMP_InputField _nameInput;
        [SerializeField] private TMP_InputField _scoreInput;
        
        void Start() {
            _manager = FirebaseManager.Instance;
        }

        public void StartGame() => SceneManager.LoadScene("Game");
        
        public void TestGetLeaderBoard() {
            _manager.GetLeaderboard(PrintLeaderBoard);
        }

        void PrintLeaderBoard(List<ScoreEntry> entries) {
            if (entries != null) {
                foreach (var e in entries) {
                    Debug.Log($"name: {e.username} | score: {e.score}");
                }
            }
        }

        public void TestAddEntry() {
            _manager.AddScoreEntry(new ScoreEntry(_nameInput.text, int.Parse(_scoreInput.text)));
        }

        public void ClearCache() {
            PlayerPrefs.SetString("REFRESH_TOKEN", string.Empty);
            PlayerPrefs.SetString("ID_TOKEN", string.Empty);
            PlayerPrefs.Save();
        }
    }
}

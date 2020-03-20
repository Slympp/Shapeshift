using TMPro;
using UnityEngine;

namespace MainMenu {
    public class LeaderboardScoreEntry : MonoBehaviour {
        [SerializeField] private TMP_Text position;
        [SerializeField] private TMP_Text username;
        [SerializeField] private TMP_Text score;

        public void SetValues(int _position, string _username, int _score) {
            position.SetText(_position.ToString());
            username.SetText(_username);
            score.SetText(_score.ToString());
        }
    }
}
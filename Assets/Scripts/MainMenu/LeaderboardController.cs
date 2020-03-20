using System.Collections;
using Firebase;
using UnityEngine;

namespace MainMenu {
    public class LeaderboardController : MonoBehaviour {

        [SerializeField] private int entriesToDisplay = 50;
        [SerializeField] private Transform listRoot;
        [SerializeField] private GameObject entryPrefab;
        [SerializeField] private GameObject loadingArrow;
        [SerializeField] private GameObject errorObject;
        [SerializeField] private float loadingTimeout = 5f;

        private Coroutine _populateLeaderboard = null;
        
        void OnEnable() {
            PopulateLeaderboard();
        }

        private void OnDisable() {
            ClearLeaderboard();
            errorObject.SetActive(false);
        }
        
        private void PopulateLeaderboard() {
            if (_populateLeaderboard != null)
                StopCoroutine(_populateLeaderboard);
            StartCoroutine(nameof(PopulateRoutine));
        }

        private IEnumerator PopulateRoutine() {
            DisplayLoading(true);

            var timeout = loadingTimeout;
            while (timeout > 0 && !FirebaseManager.Instance.IsSignedIn()) {
                timeout -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            
            if (FirebaseManager.Instance.IsSignedIn()) {
                FirebaseManager.Instance.GetLeaderboard(
                    l => {
                        DisplayLoading(false);
                        for (int i = 0; i < entriesToDisplay && i < l.Count; i++) {
                            var entry = Instantiate(entryPrefab, listRoot);
                            if (entry.TryGetComponent<LeaderboardScoreEntry>(out var e))
                                e.SetValues(i + 1, l[i].username, l[i].score);
                        }
                    }, 
                    () => DisplayError(true));
            } else
                DisplayError(true);
        }

        private void ClearLeaderboard() {
            foreach (Transform t in listRoot)
                Destroy(t.gameObject);
        }
        
        private void DisplayLoading(bool active) => loadingArrow.SetActive(active);

        private void DisplayError(bool active) {
            if (active) 
                DisplayLoading(false);
            
            errorObject.SetActive(active);
        }
        
        public void RetryRefreshLeaderboard() {
            DisplayError(false);
            DisplayLoading(true);
            ClearLeaderboard();
            PopulateLeaderboard();
        }
    }
}

using Firebase;
using UnityEngine;

namespace MainMenu {
    public class ConnectionSettingController : MonoBehaviour {
        [SerializeField] private GameObject connectedObject;
        [SerializeField] private GameObject disconnectedObject;
        [SerializeField] private GameObject loadingObject;

        private bool _connecting;
        
        private void OnEnable() {
            loadingObject.SetActive(false);
            Toggle(FirebaseManager.Instance.IsSignedIn());
        }

        public void Toggle(bool active) {
            connectedObject.SetActive(active);
            disconnectedObject.SetActive(!active);
        }
        
        public void RetryConnection() {
            if (_connecting)
                return;

            _connecting = true;
            
            connectedObject.SetActive(false);
            disconnectedObject.SetActive(false);
            loadingObject.SetActive(true);
            
            FirebaseManager.Instance.ClearCache();
            FirebaseManager.Instance.Authenticate(success => {
                loadingObject.SetActive(false);
                Toggle(success);
                _connecting = false;
            });
        }
    }
}

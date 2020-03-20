using UnityEngine;

namespace Android {
    public class MobilePauseButton : MonoBehaviour {

        private void Awake() {
#if !UNITY_ANDROID
            gameObject.SetActive(false);
#endif
        }
    }
}

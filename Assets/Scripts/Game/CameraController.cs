using Cinemachine;
using UnityEngine;

namespace Game {
    public class CameraController : MonoBehaviour {

        private CinemachineVirtualCamera _virtualCamera;

        void Awake() {
            TryGetComponent(out _virtualCamera);
        }

        public void UpdateCameraTarget(Transform target) {
            if (_virtualCamera)
                _virtualCamera.LookAt = target;
        }
    }
}
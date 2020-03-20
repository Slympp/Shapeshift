using UnityEngine;

namespace Game {
    public class CameraController : MonoBehaviour {

        [SerializeField] private float pingPongRange = 3f;
        [SerializeField] private float speed;
        
        private Transform _transform;
        private Vector3 _startingEuler;
        private bool _raising = true;

        private void Awake() {
            _transform = transform;
            _startingEuler = _transform.localRotation.eulerAngles;
        }

        public void PingPong() {
//            var rotation = _transform.rotation;
//            var movement = new Vector3(Time.unscaledDeltaTime * speed, 0, 0);
//            
//            if (_raising) {
//                rotation = Quaternion.Lerp(_transform.rotation, rotation * Quaternion.Euler(movement), speed * Time.unscaledDeltaTime * 10);
//                if (rotation.eulerAngles.x >= _startingEuler.x + pingPongRange)
//                    _raising = false;
//            }
//            else {
//                rotation = Quaternion.Lerp(_transform.rotation, rotation * Quaternion.Euler(-movement), speed * Time.unscaledDeltaTime * 10);
//                if (rotation.eulerAngles.x <= _startingEuler.x - pingPongRange)
//                    _raising = true;
//            }
        }
    }
}
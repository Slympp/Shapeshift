using UnityEngine;

namespace Game.Level {
    
    [RequireComponent(typeof(Rigidbody))]
    public class ExplodableObstacle : MonoBehaviour {

        private readonly Vector2 _minMaxExplodeDuration = new Vector2(0.1f, 0.15f);
        private readonly Vector2 _minMaxThrust = new Vector2(60, 70);
        
        private Rigidbody _rigidbody;
        private float _remainingDuration;
        private float _thrust;
        
        private Transform _transform;
        private Vector3 _cachedOriginPosition;
        private Quaternion _cachedOriginRotation;

        private void Awake() {
            _transform = transform;
            _cachedOriginPosition = _transform.localPosition;
            _cachedOriginRotation = _transform.localRotation;

            if (!TryGetComponent(out _rigidbody)) {
                Debug.LogError($"Failed to find rigidbody on {name}");
                return;
            }
            _rigidbody.isKinematic = true;
        }

        public void Explode() {
            _rigidbody.isKinematic = false;
            _remainingDuration = Random.Range(_minMaxExplodeDuration.x, _minMaxExplodeDuration.y);
            _thrust = Random.Range(_minMaxThrust.x, _minMaxThrust.y);
        }
        
        private void FixedUpdate() {
            if (_remainingDuration <= 0)
                return;
            
            _rigidbody.AddForce(new Vector3(Random.Range(-1f, 1f), 1, 1) * _thrust);
            _remainingDuration -= Time.deltaTime;
        }

        private void OnBecameInvisible() {
            gameObject.SetActive(false);
            _transform.localPosition = _cachedOriginPosition;
            _transform.localRotation = _cachedOriginRotation;
        }
    }
}

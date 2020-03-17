using System;
using UnityEngine;

namespace Game.Level {
    public class DayNightCycle : MonoBehaviour {

        [Tooltip("in seconds")]
        [SerializeField] private float fullDayTime;
        [SerializeField] private float dayTimeMultiplier = 1f;
        [SerializeField] private float nightTimeMultiplier = 2f;
        
        private Transform _transform;
        private float _speed;
        
        void Start() {
            _transform = transform;
            _speed = 360 / fullDayTime;
        }

        void Update() {
            if (GameManager.Instance.IsGamePaused || GameManager.Instance.IsGameOver)
                return;
            
            var fixedUnscaledDeltaTime = Time.deltaTime * (1 / Time.timeScale);
            var currentSpeed = _speed * (_transform.rotation.eulerAngles.x < 180 ? dayTimeMultiplier : nightTimeMultiplier);

            _transform.RotateAround(Vector3.zero, Vector3.back, fixedUnscaledDeltaTime * currentSpeed);
            _transform.LookAt(Vector3.zero);
        }
    }
}

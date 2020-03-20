using UnityEngine;

namespace Game.Level {
    public class DayNightCycle : MonoBehaviour {

        [Tooltip("in seconds")]
        [SerializeField] private float fullDayTime;
        [SerializeField] private float daySpeedMultiplier = 1f;
        [SerializeField] private float nightSpeedMultiplier = 2f;
        
        [SerializeField] private Color dayLightColor;
        [SerializeField] private Color nightLightColor;
        [SerializeField] private Vector2 lightIntensity;

        [SerializeField] private AudioSource dayAmbientSource;
        [SerializeField] private AudioSource nightAmbientSource;
        [Range(0, 1)]
        [SerializeField] private float dayMaxVolume = 1;
        [Range(0, 1)]
        [SerializeField] private float nightMaxVolume = 1;
        
        private Transform _transform;
        private Light _light;
        
        private float _speed;

        private void Awake() {
            _transform = transform;
            
            TryGetComponent(out _light);
        }

        void Start() {
            _speed = 360 / fullDayTime;
        }

        void Update() {
            if (GameManager.Instance.IsGamePaused || GameManager.Instance.IsGameOver)
                return;
            
            var fixedUnscaledDeltaTime = Time.deltaTime * (1 / Time.timeScale);

            var xAngle = _transform.rotation.eulerAngles.x;
            var isDayTime = xAngle < 180;
            
            var currentSpeed = _speed * (isDayTime ? daySpeedMultiplier : nightSpeedMultiplier);
            _transform.RotateAround(Vector3.zero, Vector3.back, fixedUnscaledDeltaTime * currentSpeed);
            _transform.LookAt(Vector3.zero);

            float v, lIntensity, lColor;
            if (isDayTime) {
                v = xAngle.Normalize(0, 60);
                lIntensity = v.Normalize(0, 1, (lightIntensity.x + lightIntensity.y) / 2, lightIntensity.y);
                lColor = v.Normalize(0, 1, .5f, 1);
                dayAmbientSource.volume = v.Normalize(0, 1, 0, dayMaxVolume);
            } else {
                v = xAngle.Normalize(360, 300);
                lIntensity = v.Normalize(0, 1, (lightIntensity.x + lightIntensity.y) / 2, lightIntensity.x);
                lColor = v.Normalize(0, 1, .5f, 0);
                nightAmbientSource.volume = v.Normalize(0, 1, 0, nightMaxVolume);
            }
            
            _light.intensity = lIntensity;
            _light.color = Color.Lerp(nightLightColor, dayLightColor, lColor);
        }
    }
}

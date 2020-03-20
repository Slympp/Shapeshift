using System.Collections;
using System.Linq;
using DG.Tweening;
using Firebase;
using Game.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game {
    public class GameManager : MonoBehaviour {

        public static GameManager Instance { get; private set; }
        public float GameDuration { get; private set; }
        public bool IsGameOver { get; private set; }
        public bool IsGamePaused { get; private set; }

        [Header("Settings")]
        
        [Tooltip("(in seconds)")]
        [SerializeField] private Vector2 increaseDifficultyTimeframe;

        [Tooltip("x: min speed, y: max speed")]
        [SerializeField] private Vector2 speed;

        [SerializeField] private float scoreMultiplier;
        
        [SerializeField] private float shapeShiftLockDuration = 0.4f;
        
        [SerializeField] private PlayerController playerController;
        [SerializeField] private UIController uiController;
        [SerializeField] private CameraController cameraController;

        private bool _shapeShiftLocked;
        private bool _rotationDone = true;
        
        private float _pausedTimeScale = 1;
        private bool _restarting;
        private float _score;
        
        void Awake() {
            if (Instance != null) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DOTween.Init();
        }

        void Start() {
            StartCoroutine(uiController.ToggleBlackBorders(false));
        }

        void Update() {

            UpdateGameControlInputs();

            if (IsGameOver || IsGamePaused)
                return;
            
            UpdatePlayerInputs();
            
            GameDuration += Time.unscaledDeltaTime;
            
            var speedMultiplier = GetGameSpeedMultiplier();
            Time.timeScale = speedMultiplier;
            
            _score += Time.deltaTime * speedMultiplier * scoreMultiplier;
            uiController.UpdateScoreText(_score, speedMultiplier);
            
            cameraController.PingPong();
        }

        void UpdateGameControlInputs() {
            if (InputManager.Pause) {
                if (IsGameOver) {
                    LoadScene("Game");
                } else {
                    if (IsGamePaused)
                        Resume();
                    else
                        Pause();
                }
            }
        }

        void UpdatePlayerInputs() {
            if (_shapeShiftLocked || !_rotationDone)
                return;

            if (InputManager.ShapeShiftPrevious) {
                _shapeShiftLocked = true;
                _rotationDone = false;

                StartCoroutine(nameof(ShapeShiftLockTimer));
                StartCoroutine(playerController.ShapeShift(-1, shapeShiftLockDuration));
                
                uiController.RotateShapeShiftHUD(false, shapeShiftLockDuration, () => _rotationDone = true);

            } else if (InputManager.ShapeShiftNext) {
                _shapeShiftLocked = true;
                _rotationDone = false;
                
                StartCoroutine(nameof(ShapeShiftLockTimer));
                StartCoroutine(playerController.ShapeShift(1, shapeShiftLockDuration));
                
                uiController.RotateShapeShiftHUD(true, shapeShiftLockDuration, () => _rotationDone = true);
            }
        }

        IEnumerator ShapeShiftLockTimer() {
            yield return new WaitForSecondsRealtime(shapeShiftLockDuration);
            _shapeShiftLocked = false;
        }
        
        float GetGameSpeedMultiplier() {
            if (GameDuration < increaseDifficultyTimeframe.x)
                return speed.x;
            if (GameDuration > increaseDifficultyTimeframe.y)
                return speed.y;
            
            return (GameDuration - increaseDifficultyTimeframe.x) 
                   / (increaseDifficultyTimeframe.y - increaseDifficultyTimeframe.x) 
                   * (speed.y - speed.x) + speed.x;
        }

        #region GAME_CONTROLS
        
        public void GameOver() {
            if (!IsGameOver) {
                IsGameOver = true;
                Time.timeScale = 0;
                
                playerController.DisableCurrentShape();

                uiController.ToggleHUD(false);
                uiController.TogglePausePanel(false);
                uiController.ToggleGameOverPanel(true);
                
                var score = Mathf.RoundToInt(_score);
                uiController.SetGameOverScore(score);
                
                if (FirebaseManager.Instance.IsSignedIn()) {
                    FirebaseManager.Instance.AddScoreEntry(new ScoreEntry(PlayerPrefs.GetString(SavedKeys.SavedUsername), score),
                        () => {
                            FirebaseManager.Instance.GetLeaderboard(
                                leaderboard => { 
                                    var position = leaderboard.FindIndex(entry => entry.score == score);
                                    var percentile = 100f - (float)position / leaderboard.Count * 100f;
                                    
                                    uiController.SetGameOverPosition(position + 1, percentile);
                                    
                                }, uiController.DisplayConnectionError);
                        }, uiController.DisplayConnectionError);
                } else
                    uiController.DisplayConnectionError();
            }
        }

        public void LoadScene(string sceneName) {
            if (_restarting)
                return;

            _restarting = true;
            StartCoroutine(uiController.ToggleBlackBorders(true, () => {
                Time.timeScale = 1;
                SceneManager.LoadScene(sceneName);
            }));
        }

        public void Pause() {
            IsGamePaused = true;
            
            uiController.TogglePausePanel(true);
            uiController.ToggleHUD(false);

            _pausedTimeScale = Time.timeScale;
            Time.timeScale = 0;
        }

        public void Resume() {
            IsGamePaused = false;
            
            uiController.TogglePausePanel(false);
            uiController.ToggleHUD(true);
            
            Time.timeScale = _pausedTimeScale;
        }
        
        #endregion
    }
}
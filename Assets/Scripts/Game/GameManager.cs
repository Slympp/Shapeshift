using System.Collections;
using Game.UI;
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

        private bool _shapeShiftLocked;
        private float _pausedTimeScale = 1;
        private float _score;
        
        private void Awake() {
            
            if (Instance != null)
                Destroy(gameObject);

            Instance = this;
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
        }

        void UpdateGameControlInputs() {
            if (InputManager.Pause) {
                if (IsGameOver) {
                    RestartGame();
                } else {
                    if (IsGamePaused)
                        Resume();
                    else
                        Pause();
                }
            }
        }

        void UpdatePlayerInputs() {
            if (_shapeShiftLocked)
                return;

            if (InputManager.ShapeShiftPrevious) {
                _shapeShiftLocked = true;

                StartCoroutine(nameof(ShapeShiftLockTimer));
                StartCoroutine(playerController.ShapeShift(-1, shapeShiftLockDuration));
                
                uiController.RotateHUD(false, shapeShiftLockDuration);

            } else if (InputManager.ShapeShiftNext) {
                _shapeShiftLocked = true;
                
                StartCoroutine(nameof(ShapeShiftLockTimer));
                StartCoroutine(playerController.ShapeShift(1, shapeShiftLockDuration));
                
                uiController.RotateHUD(true, shapeShiftLockDuration);
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
                
                uiController.ToggleShapeShiftHUD(false);
                uiController.TogglePausePanel(false);
                uiController.ToggleGameOverPanel(true);
            }
        }

        public void RestartGame() {
            SceneManager.LoadScene("Game");
            Time.timeScale = 1;
        }

        void Pause() {
            IsGamePaused = true;
            
            uiController.TogglePausePanel(true);
            uiController.ToggleShapeShiftHUD(false);

            _pausedTimeScale = Time.timeScale;
            Time.timeScale = 0;
        }

        void Resume() {
            IsGamePaused = false;
            
            uiController.TogglePausePanel(false);
            uiController.ToggleShapeShiftHUD(true);
            
            Time.timeScale = _pausedTimeScale;
        }
        
        #endregion
    }
}
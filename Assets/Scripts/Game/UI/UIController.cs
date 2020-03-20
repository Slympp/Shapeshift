using System;
using System.Collections;
using Audio;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Game.UI {
    // ReSharper disable once InconsistentNaming
    public class UIController : MonoBehaviour {
        
        [SerializeField] private ShapeShiftHUD shapeShiftHud;
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject mobilePauseButton;
        
        [Header("GameOver")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TMP_Text gameOverScoreText;
        [SerializeField] private GameObject positionLoadingObj;
        [SerializeField] private GameObject positionInfosObj;
        [SerializeField] private TMP_Text positionText;
        [SerializeField] private TMP_Text positionPercentileText;
        [SerializeField] private GameObject positionErrorObj;
        
        [Header("Score Text")]
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text scoreMultiplierText;
        [SerializeField] private GameObject scoreMultiplierObject;
        [SerializeField] private float scoreMultiplierOnAnimMaxScale = 0.8f;

        [Header("Black borders")]
        [SerializeField] private RectTransform topBlackBorder;
        [SerializeField] private RectTransform bottomBlackBorder;
        [SerializeField] private float borderAnimationDuration;

        private int _lastScoreMultiplier = 1;

        // ReSharper disable once InconsistentNaming
        public void ToggleHUD(bool active) {
#if UNITY_ANDROID
            mobilePauseButton.SetActive(active);
#endif
            shapeShiftHud.transform.parent.gameObject.SetActive(active);
        }

        // ReSharper disable once InconsistentNaming
        public void RotateShapeShiftHUD(bool rightDirection, float duration, Action onRotationComplete) {
            StartCoroutine(shapeShiftHud.Rotate(rightDirection, duration, onRotationComplete));
        }

        public void UpdateScoreText(float value, float multiplier) {
            scoreText.SetText("<mspace=0.6em>" + Mathf.RoundToInt(value) + "</mspace>");

            var v = Mathf.FloorToInt(multiplier);
            if (v > _lastScoreMultiplier) {
                _lastScoreMultiplier = v;
                
                if (!scoreMultiplierObject.activeSelf) scoreMultiplierObject.SetActive(true);
                
                AudioManager.Instance.PlayOnScoreIncrement();

                var initialScale = scoreMultiplierObject.transform.localScale;
                scoreMultiplierObject.transform.DOScale(scoreMultiplierOnAnimMaxScale, .2f).OnComplete(() => scoreMultiplierObject.transform.DOScale(initialScale, .8f));
                scoreMultiplierText.SetText("<size=80%>x</size>" + _lastScoreMultiplier);
            }
        }
        
        public void TogglePausePanel(bool active) {
            scoreText.gameObject.SetActive(!active);
            shapeShiftHud.gameObject.SetActive(!active);
            pausePanel.SetActive(active);
        }
        
        public void ToggleGameOverPanel(bool active) {
            scoreText.gameObject.SetActive(!active);
            shapeShiftHud.gameObject.SetActive(!active);
            gameOverPanel.SetActive(active);
        }

        public void SetGameOverScore(int score) => gameOverScoreText.SetText(score + " points");

        public void SetGameOverPosition(int position, float percentile) {
            positionLoadingObj.SetActive(false);
            
            positionText.SetText("#" + position);
            positionPercentileText.SetText($"(better than {percentile:0.00}% other players)");
            
            positionInfosObj.SetActive(true);
        }

        public void DisplayConnectionError() {
            positionLoadingObj.SetActive(false);
            positionErrorObj.SetActive(true);
        }

        public IEnumerator ToggleBlackBorders(bool active, Action onCompleted = null) {
            var size = active ? 540 : 100;
            var topDone = false;
            var botDone = false;
            
            topBlackBorder.DOSizeDelta(new Vector2(0, size), borderAnimationDuration).SetUpdate(UpdateType.Normal, true).OnComplete(() => topDone = true);
            bottomBlackBorder.DOSizeDelta(new Vector2(0, size), borderAnimationDuration).SetUpdate(UpdateType.Normal, true).OnComplete(() => botDone = true);
            
            yield return new WaitUntil(() => topDone && botDone);
            onCompleted?.Invoke();
        }

        public void PlayButtonClickSound() => AudioManager.Instance.PlayOnButtonClicked();
    }
}
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Game.UI {
    public class UIController : MonoBehaviour {
        
        [SerializeField] private ShapeShiftHUD shapeShiftHud;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject pausePanel;
        
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text scoreMultiplierText;
        [SerializeField] private GameObject scoreMultiplierObject;
        
        private int _lastScoreMultiplier = 1;

        // ReSharper disable once InconsistentNaming
        public void ToggleShapeShiftHUD(bool active) => shapeShiftHud.transform.parent.gameObject.SetActive(active);

        // ReSharper disable once InconsistentNaming
        public void RotateHUD(bool rightDirection, float duration) => StartCoroutine(shapeShiftHud.Rotate(rightDirection, duration));

        public void ToggleGameOverPanel(bool active) => gameOverPanel.SetActive(active);

        public void TogglePausePanel(bool active) => pausePanel.SetActive(active);

        public void UpdateScoreText(float value, float multiplier) {
            scoreText.SetText("<mspace=0.6em>" + Mathf.RoundToInt(value) + "</mspace>");

            var v = Mathf.FloorToInt(multiplier);
            if (v > _lastScoreMultiplier) {
                _lastScoreMultiplier = v;
                
                if (!scoreMultiplierObject.activeSelf) scoreMultiplierObject.SetActive(true);

                scoreMultiplierObject.transform.DOScale(1, .2f).OnComplete(() => scoreMultiplierObject.transform.DOScale(.7f, .8f));
                scoreMultiplierText.SetText("<size=80%>x</size>" + _lastScoreMultiplier);
            }
        }
    }
}
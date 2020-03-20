using System.Collections.Generic;
using Audio;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;

namespace MainMenu {
    public class QualitySettingsController : MonoBehaviour, IPointerClickHandler {

        [SerializeField] private List<UniversalRenderPipelineAsset> qualityAssets;
        [SerializeField] private TMP_Text qualityText;

        private QualityEnum _currentQuality;
        
        void OnEnable() {
            GetCurrentQuality();
            UpdateQualityText();
        }
        
        public void OnPointerClick(PointerEventData eventData) {
            switch (eventData.button) {
                case PointerEventData.InputButton.Left:
                    AudioManager.Instance.PlayOnButtonClicked();
                    UpdateQualityValue(1);
                    break;
                case PointerEventData.InputButton.Right:
                    AudioManager.Instance.PlayOnButtonClicked();
                    UpdateQualityValue(-1);
                    break;
            }
        }

        private void GetCurrentQuality() {
            for (var i = 0; i < qualityAssets.Count; i++)
                if (qualityAssets[i] == QualitySettings.renderPipeline)
                    _currentQuality = (QualityEnum)i; 
        }

        public void UpdateQualityValue(int v) {
            if (_currentQuality + v < QualityEnum.Low)
                _currentQuality = QualityEnum.High;
            else if (_currentQuality + v > QualityEnum.High)
                _currentQuality = QualityEnum.Low;
            else
                _currentQuality += v;
            
            var quality = (int) _currentQuality;
            
            if (quality < qualityAssets.Count) {
                QualitySettings.SetQualityLevel(quality, false);
                QualitySettings.renderPipeline = qualityAssets[quality];
                UpdateQualityText();
            }
        }

        private void UpdateQualityText() => qualityText.SetText("Quality : " + _currentQuality);
        
        enum QualityEnum {
            Low = 0,
            Medium = 1,
            High = 2
        }
    }
}

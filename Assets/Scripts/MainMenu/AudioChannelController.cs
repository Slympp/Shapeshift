using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu {
    public class AudioChannelController : MonoBehaviour {

        [SerializeField] private List<AudioMixerGroupObject> _mixerObjects;
        [SerializeField] private Sprite _enabled;
        [SerializeField] private Sprite _disabled;
        
        private Image _image;

        void Awake() => _image = GetComponentInChildren<Image>();
        private void OnEnable() => SetActive(IsActive);

        private bool IsActive => _mixerObjects.Count > 0 && _mixerObjects.First().isActive;
        
        void SetActive(bool b) {
            foreach (var m in _mixerObjects)
                m.Toggle(b);
            
            if (_image != null)
                _image.sprite = b ? _enabled : _disabled;
        }

        public void ToggleActive() {
            SetActive(!IsActive);
        }
    }
}

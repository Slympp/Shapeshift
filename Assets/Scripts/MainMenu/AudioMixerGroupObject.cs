using UnityEngine;
using UnityEngine.Audio;

namespace MainMenu {
    
    [CreateAssetMenu(menuName = "AudioMixerGroupObject")]
    public class AudioMixerGroupObject : ScriptableObject {

        public bool isActive = true;
        
        [SerializeField] private AudioMixer mixer;
        [SerializeField] private string _mixerName;
        [SerializeField] private float maxVolume;
        [SerializeField] private float minVolume = -80f;

        public void Toggle(bool active) {
            isActive = active;
            mixer.SetFloat(_mixerName + "Volume", isActive ? maxVolume : minVolume);
        }
    }
}

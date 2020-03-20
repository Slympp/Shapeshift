using UnityEngine;

namespace Audio {
    [RequireComponent(typeof(AudioSource))]
    public class AudioEagleStepController : MonoBehaviour {
    
        [SerializeField] private AudioClip flap;

        private AudioSource _source;

        private void Awake() {
            TryGetComponent(out _source);
        }

        // Called by Animator's keyframe event
        public void PlayFlap() {
            _source.PlayOneShot(flap);
        }
    }
}
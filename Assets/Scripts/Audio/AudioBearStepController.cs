using UnityEngine;

namespace Audio {
    
    [RequireComponent(typeof(AudioSource))]
    public class AudioBearStepController : MonoBehaviour {

        [SerializeField] private AudioClip leftStep;
        [SerializeField] private AudioClip rightStep;

        private AudioSource _source;

        private void Awake() {
            TryGetComponent(out _source);
        }

        // Called by Animator's keyframe event
        public void PlayLeftStep() => _source.PlayOneShot(leftStep);
        public void PlayRightStep() => _source.PlayOneShot(rightStep);
    }
}
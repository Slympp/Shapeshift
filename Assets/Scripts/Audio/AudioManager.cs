using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// ReSharper disable InconsistentNaming

namespace Audio {
    public class AudioManager : MonoBehaviour {

        public static AudioManager Instance { get; private set; }

        [Header("Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource uiSource;

        [Header("Music")] 
        [SerializeField] private AudioClip gameMusic;
        [SerializeField] private AudioClip mainMenuMusic;
        
        [Header("SFX")]
        [SerializeField] private AudioClip onBypassOstacle;
        [SerializeField] private AudioClip onHitObstacle;
        [SerializeField] private AudioClip onShapeshift;
        
        [Header("UI")]
        [SerializeField] private AudioClip onScoreIncrement;
        [SerializeField] private AudioClip onButtonClick;

        private void OnEnable() {
            if (Instance != null) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        #region MUSIC
        
        void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            musicSource.clip = scene.name == "Game" ? gameMusic : mainMenuMusic;
            
            if (!musicSource.mute)
                musicSource.Play();
        }

        public void ToggleMusic(bool active) => musicSource.mute = !active;

        #endregion
       
        #region SFX
        public void PlayOnBypassObstacleSFX() => PlaySFX(onBypassOstacle);
        public void PlayOnHitObstacleSFX() => PlaySFX(onHitObstacle);
        public void PlayOnShapeshiftSFX() => PlaySFX(onShapeshift);
        
        private void PlaySFX(AudioClip clip) => sfxSource.PlayOneShot(clip);
        
        #endregion

        #region UI
        
        public void PlayOnScoreIncrement() => PlayUISound(onScoreIncrement);
        public void PlayOnButtonClicked() => PlayUISound(onButtonClick);

        private void PlayUISound(AudioClip clip) => uiSource.PlayOneShot(clip);
        
        #endregion
    }
}

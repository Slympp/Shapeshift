using System;
using System.Collections;
using Audio;
using DG.Tweening;
using Firebase;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MainMenu {
    public class MainMenuManager : MonoBehaviour {

        [Header("Fade screen")]
        [SerializeField] private float connectionTimeoutDuration;
        [SerializeField] private Image fadeScreen;
        [SerializeField] private float fadeDuration;
        [SerializeField] private float fadeEndValue;
        [SerializeField] private GameObject connectingText;
        [SerializeField] private GameObject connectionErrorText;
        [SerializeField] private float connectionErrorDisplayTime = 2f;

        [Header("Panels")]
        [SerializeField] private GameObject mainScreenPanel;
        [SerializeField] private GameObject leaderboardPanel;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject nameInputPanel;

        [Header("Name input field")]
        [SerializeField] private TMP_InputField nameInputField;
        [SerializeField] private Button nameInputStartButton;
        
        private bool _inputFieldIsValid = false;

        private FirebaseManager _manager;

        IEnumerator Start() {
            fadeScreen.gameObject.SetActive(true);
            _manager = FirebaseManager.Instance;

            yield return Connect();

            if (!_manager.IsSignedIn()) {
                connectionErrorText.SetActive(true);
                yield return new WaitForSecondsRealtime(connectionErrorDisplayTime);
                connectionErrorText.SetActive(false);
            }
            
            nameInputField.onValueChanged.AddListener(OnInputFieldEdited);
            
            FadeIn();
        }

        IEnumerator Connect() {
            var timeout = 0f;
            while (timeout < connectionTimeoutDuration && !_manager.IsSignedIn()) {
                timeout += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }

        private void FadeIn() {
            connectingText.SetActive(false);
            fadeScreen.raycastTarget = false;
            
            fadeScreen.DOFade(0, fadeDuration).OnComplete(() => fadeScreen.gameObject.SetActive(false));
        }

        private void FadeOut(Action onCompleted) => fadeScreen.DOFade(fadeEndValue, fadeDuration).OnComplete(() => { onCompleted?.Invoke(); });

        public void TogglePanel(int panel) {
            var type = (PanelType) panel;
            
            mainScreenPanel.SetActive(type == PanelType.Main);
            leaderboardPanel.SetActive(type == PanelType.Leaderboard);
            settingsPanel.SetActive(type == PanelType.Settings);
            nameInputPanel.SetActive(type == PanelType.NameInput);
            PlayOnButtonClicked();
        }

        public void SetSavedUsername() {
            nameInputField.text = PlayerPrefs.GetString(SavedKeys.SavedUsername);
            OnInputFieldEdited(nameInputField.text);
        }

        private void OnInputFieldEdited(string value) {
            _inputFieldIsValid = !string.IsNullOrWhiteSpace(value) && value.Length > 0 && value.Length < 60;
            nameInputStartButton.interactable = _inputFieldIsValid;
        }

        public void StartGame() {
            if (!_inputFieldIsValid) return;
            
            PlayerPrefs.SetString(SavedKeys.SavedUsername, nameInputField.text);
            PlayerPrefs.Save();
                
            FadeOut(() => SceneManager.LoadScene("Game"));
        }

        public void PlayOnButtonClicked() => AudioManager.Instance.PlayOnButtonClicked();
    }
    
    public enum PanelType {
        Main = 0,
        Leaderboard = 1,
        Settings = 2,
        NameInput = 3
    }
}

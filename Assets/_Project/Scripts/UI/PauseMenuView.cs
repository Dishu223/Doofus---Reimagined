using System;
using TileGame.Core;
using UnityEngine;
using UnityEngine.UI;

namespace TileGame.UI
{
    public class PauseMenuView : MonoBehaviour, IView
    {
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _restartButton;

        public event Action OnResumeClicked;
        public event Action OnRestartClicked;

        private void Awake()
        {
            _resumeButton.onClick.AddListener(HandleResume);
            _restartButton.onClick.AddListener(HandleRestart);
        }

        private void OnDestroy()
        {
            _resumeButton.onClick.RemoveListener(HandleResume);
            _restartButton.onClick.RemoveListener(HandleRestart);
        }

        private void HandleResume() => OnResumeClicked?.Invoke();
        private void HandleRestart() => OnRestartClicked?.Invoke();

        public void SetVisible(bool visible) => gameObject.SetActive(visible);
    }
}
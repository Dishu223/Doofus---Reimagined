using System;
using TMPro;
using TileGame.Core;
using UnityEngine;
using UnityEngine.UI;

namespace TileGame.UI
{
    public class HUDView : MonoBehaviour, IView
    {
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private Button _pauseButton;

        public event Action OnPauseClicked;

        private void Awake()
        {
            if (_pauseButton != null)
                _pauseButton.onClick.AddListener(HandlePauseClicked);
        }

        private void OnDestroy()
        {
            if (_pauseButton != null)
                _pauseButton.onClick.RemoveListener(HandlePauseClicked);
        }

        private void HandlePauseClicked() => OnPauseClicked?.Invoke();

        public void UpdateScoreText(int current, int target)
        {
            _scoreText.SetText("Tiles: {0} / {1}".AsSpan(), current, target);
        }

        public void SetVisible(bool visible) => gameObject.SetActive(visible);
    }
}
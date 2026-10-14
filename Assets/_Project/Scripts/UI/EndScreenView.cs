using System;
using TMPro;
using TileGame.Core;
using UnityEngine;
using UnityEngine.UI;

namespace TileGame.UI
{
    public class EndScreenView : MonoBehaviour, IView
    {
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _finalScoreText;
        [SerializeField] private Button _restartButton;

        public event Action OnRestartClicked;

        private void Awake()
        {
            _restartButton.onClick.AddListener(HandleRestart);
        }

        private void OnDestroy()
        {
            _restartButton.onClick.RemoveListener(HandleRestart);
        }

        private void HandleRestart() => OnRestartClicked?.Invoke();

        public void ShowResult(bool isVictory, int finalScore, int targetScore)
        {
            _titleText.text = isVictory ? "VICTORY!" : "GAME OVER";
            _titleText.color = isVictory ? new Color(0.2f, 0.9f, 0.4f) : new Color(1f, 0.3f, 0.3f);
            _finalScoreText.SetText("Stepped on {0} / {1} Tiles".AsSpan(), finalScore, targetScore);
            gameObject.SetActive(true);
        }

        public void SetVisible(bool visible) => gameObject.SetActive(visible);
    }
}
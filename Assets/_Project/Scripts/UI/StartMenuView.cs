using System;
using TileGame.Core;
using UnityEngine;
using UnityEngine.UI;

namespace TileGame.UI
{
    public class StartMenuView : MonoBehaviour, IView
    {
        [SerializeField] private Button _startButton;

        public event Action OnStartClicked;

        private void Awake()
        {
            _startButton.onClick.AddListener(HandleStartClicked);
        }

        private void OnDestroy()
        {
            _startButton.onClick.RemoveListener(HandleStartClicked);
        }

        private void HandleStartClicked() => OnStartClicked?.Invoke();

        public void SetVisible(bool visible) => gameObject.SetActive(visible);
    }
}
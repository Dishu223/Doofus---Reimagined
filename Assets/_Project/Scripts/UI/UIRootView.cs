using TileGame.Core;
using UnityEngine;

namespace TileGame.UI
{
    public class UIRootView : MonoBehaviour, IView
    {
        [SerializeField] private StartMenuView _startMenu;
        [SerializeField] private HUDView _hud;
        [SerializeField] private PauseMenuView _pauseMenu;
        [SerializeField] private EndScreenView _endScreen;

        public StartMenuView StartMenu => _startMenu;
        public HUDView HUD => _hud;
        public PauseMenuView PauseMenu => _pauseMenu;
        public EndScreenView EndScreen => _endScreen;
    }
}
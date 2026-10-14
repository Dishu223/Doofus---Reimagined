using System;
using TileGame.Core;

namespace TileGame.UI
{
    /// <summary>
    /// Pure C# MVC Controller for all UI screens (Start, HUD, Pause, Game Over/Victory).
    /// Listens to UI View button events and updates the UI whenever GameStateModel changes.
    /// </summary>
    public class UIController : BaseController<GameStateModel, UIRootView>
    {
        public event Action OnStartGameRequested;
        public event Action OnResumeGameRequested;
        public event Action OnPauseGameRequested;
        public event Action OnRestartGameRequested;

        public UIController(GameStateModel model, UIRootView view) : base(model, view)
        {
            // 1. Subscribe to Model changes
            Model.OnScoreChanged += HandleScoreChanged;
            Model.OnStateChanged += HandleStateChanged;

            // 2. Subscribe to View button clicks
            View.StartMenu.OnStartClicked += HandleStartClicked;
            View.HUD.OnPauseClicked += HandlePauseClicked;
            View.PauseMenu.OnResumeClicked += HandleResumeClicked;
            View.PauseMenu.OnRestartClicked += HandleRestartClicked;
            View.EndScreen.OnRestartClicked += HandleRestartClicked;

            // 3. Initialize starting UI state
            HandleScoreChanged(Model.CurrentScore, Model.TargetScore);
            HandleStateChanged(Model.CurrentState);
        }

        private void HandleScoreChanged(int current, int target)
        {
            View.HUD.UpdateScoreText(current, target);
        }

        private void HandleStateChanged(GameState state)
        {
            // Toggle visibility of all 4 screens based on current state
            View.StartMenu.SetVisible(state == GameState.MainMenu);
            View.HUD.SetVisible(state == GameState.Playing || state == GameState.Paused);
            View.PauseMenu.SetVisible(state == GameState.Paused);

            if (state == GameState.GameOver || state == GameState.Victory)
            {
                View.EndScreen.ShowResult(
                    isVictory: state == GameState.Victory,
                    finalScore: Model.CurrentScore,
                    targetScore: Model.TargetScore
                );
            }
            else
            {
                View.EndScreen.SetVisible(false);
            }
        }

        private void HandleStartClicked() => OnStartGameRequested?.Invoke();
        private void HandlePauseClicked() => OnPauseGameRequested?.Invoke();
        private void HandleResumeClicked() => OnResumeGameRequested?.Invoke();
        private void HandleRestartClicked() => OnRestartGameRequested?.Invoke();

        public override void Dispose()
        {
            Model.OnScoreChanged -= HandleScoreChanged;
            Model.OnStateChanged -= HandleStateChanged;

            View.StartMenu.OnStartClicked -= HandleStartClicked;
            View.HUD.OnPauseClicked -= HandlePauseClicked;
            View.PauseMenu.OnResumeClicked -= HandleResumeClicked;
            View.PauseMenu.OnRestartClicked -= HandleRestartClicked;
            View.EndScreen.OnRestartClicked -= HandleRestartClicked;
        }
    }
}
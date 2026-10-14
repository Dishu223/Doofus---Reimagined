using System;
using TileGame.Configs;
using TileGame.Core;

namespace TileGame.UI
{
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver,
        Victory
    }

    /// <summary>
    /// Pure C# Model holding Score and Game Flow State.
    /// Fires events whenever data changes so Controllers can update Views.
    /// </summary>
    public class GameStateModel : IModel
    {
        public int TargetScore { get; }
        public int CurrentScore { get; private set; }
        public GameState CurrentState { get; private set; }

        public event Action<int, int> OnScoreChanged; // (currentScore, targetScore)
        public event Action<GameState> OnStateChanged;

        public GameStateModel(GameConfigSO config)
        {
            TargetScore = config.TargetWinScore;
            CurrentScore = 0;
            CurrentState = GameState.MainMenu;
        }

        public void ResetScore()
        {
            CurrentScore = 0;
            OnScoreChanged?.Invoke(CurrentScore, TargetScore);
        }

        public void IncrementScore()
        {
            if (CurrentState != GameState.Playing) return;

            CurrentScore++;
            OnScoreChanged?.Invoke(CurrentScore, TargetScore);

            if (CurrentScore >= TargetScore)
            {
                SetState(GameState.Victory);
            }
        }

        public void SetState(GameState newState)
        {
            if (CurrentState == newState) return;
            CurrentState = newState;
            OnStateChanged?.Invoke(CurrentState);
        }
    }
}
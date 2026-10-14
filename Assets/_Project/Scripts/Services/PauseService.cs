using TileGame.Core;
using UnityEngine;

namespace TileGame.Services
{
    /// Controls global pause state and physics timeScale so nothing moves or counts down while paused.
    public class PauseService : IPauseService
    {
        public bool IsPaused { get; private set; }

        public void SetPaused(bool isPaused)
        {
            IsPaused = isPaused;
            // Setting timeScale to 0 freezes Unity's Physics/Rigidbody engine completely
            Time.timeScale = isPaused ? 0f : 1f;
        }
    }
}
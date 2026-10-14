using System;
using TileGame.Core;
using UnityEngine;

namespace TileGame.Player
{
    public class PlayerController : BaseController<PlayerModel, PlayerView>, ITickable
    {
        private readonly IInputService _inputService;

        // Fires when the cube falls below FallThresholdY
        public event Action OnPlayerFell;

        public PlayerController(PlayerModel model, PlayerView view, IInputService inputService) 
            : base(model, view)
        {
            _inputService = inputService;
        }

        public void SpawnPlayer()
        {
            Model.IsActive = true;
            View.ResetToPosition(Model.StartPosition);
        }

        public void SetActive(bool active)
        {
            Model.IsActive = active;
            if (!active)
            {
                View.SetHorizontalVelocity(Vector2.zero, 0f);
            }
        }

        public void Tick(float deltaTime)
        {
            if (!Model.IsActive) return;

            // 1. Read WASD input and command the View to move
            Vector2 input = _inputService.MoveInput;
            View.SetHorizontalVelocity(input, Model.MoveSpeed);

            // 2. Check lose condition: did the player fall off the tiles?
            if (View.CurrentPosition.y < Model.FallThresholdY)
            {
                Model.IsActive = false;
                OnPlayerFell?.Invoke();
            }
        }

        public override void Dispose()
        {
            OnPlayerFell = null;
        }
    }
}
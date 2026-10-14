using TileGame.Configs;
using TileGame.Core;
using UnityEngine;

namespace TileGame.Player
{
    public class PlayerModel : IModel
    {
        public float MoveSpeed { get; }
        public Vector3 StartPosition { get; }
        public float FallThresholdY { get; }
        public bool IsActive { get; set; }

        public PlayerModel(PlayerConfigSO config)
        {
            MoveSpeed = config.MoveSpeed;
            StartPosition = config.StartPosition;
            FallThresholdY = config.FallThresholdY;
            IsActive = false;
        }
    }
}
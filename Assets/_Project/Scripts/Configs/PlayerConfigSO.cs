using UnityEngine;

namespace TileGame.Configs
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "TileGame/Configs/Player Config")]
    public class PlayerConfigSO : ScriptableObject
    {
        [Header("Movement")]
        [Tooltip("Movement speed of the player cube on X and Z axes.")]
        [SerializeField] private float _moveSpeed = 7f;

        [Header("Spawn & Death")]
        [Tooltip("Starting position of the player when the game begins.")]
        [SerializeField] private Vector3 _startPosition = new Vector3(0f, 1.5f, 0f);

        [Tooltip("If Player Y position drops below this value, trigger Game Over.")]
        [SerializeField] private float _fallThresholdY = -4f;

        public float MoveSpeed => _moveSpeed;
        public Vector3 StartPosition => _startPosition;
        public float FallThresholdY => _fallThresholdY;
    }
}
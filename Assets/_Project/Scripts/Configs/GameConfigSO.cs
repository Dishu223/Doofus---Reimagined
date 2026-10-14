using UnityEngine;

namespace TileGame.Configs
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "TileGame/Configs/Game Config")]
    public class GameConfigSO : ScriptableObject
    {
        [Tooltip("Number of unique tiles the player must step on to win.")]
        [SerializeField] private int _targetWinScore = 50;

        public int TargetWinScore => _targetWinScore;
    }
}
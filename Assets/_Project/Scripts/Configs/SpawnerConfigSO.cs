using UnityEngine;

namespace TileGame.Configs
{
    [CreateAssetMenu(fileName = "SpawnerConfig", menuName = "TileGame/Configs/Spawner Config")]
    public class SpawnerConfigSO : ScriptableObject
    {
        [Header("Wave Settings")]
        [Tooltip("Seconds between each tile spawn wave.")]
        [SerializeField] private float _spawnInterval = 1.8f;

        [Tooltip("Maximum number of tiles that can spawn at once per wave (up to 2).")]
        [Range(1, 2)]
        [SerializeField] private int _maxTilesPerWave = 2;

        [Tooltip("How long each tile stays alive before vanishing (in seconds).")]
        [SerializeField] private float _tileLifetime = 5f;

        [Header("Pooling & Placement")]
        [Tooltip("How many tiles to pre-warm in the Object Pool at startup.")]
        [SerializeField] private int _initialPoolSize = 12;

        [Tooltip("Minimum and Maximum distance from the last tile so jumps are always reachable!")]
        [SerializeField] private float _minSpawnRadius = 2.5f;
        [SerializeField] private float _maxSpawnRadius = 4.5f;

        public float SpawnInterval => _spawnInterval;
        public int MaxTilesPerWave => _maxTilesPerWave;
        public float TileLifetime => _tileLifetime;
        public int InitialPoolSize => _initialPoolSize;
        public float MinSpawnRadius => _minSpawnRadius;
        public float MaxSpawnRadius => _maxSpawnRadius;
    }
}
using System;
using System.Collections.Generic;
using TileGame.Configs;
using TileGame.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TileGame.Tiles
{
    public class TileSpawnerController : ITickable, IDisposable
    {
        private readonly SpawnerConfigSO _config;
        private readonly IObjectPool<TileView> _tileViewPool;

        private readonly List<TileController> _activeControllers = new List<TileController>();
        private readonly Dictionary<TileView, TileController> _controllerLookup = new Dictionary<TileView, TileController>();
        private readonly HashSet<Vector2Int> _occupiedGridCells = new HashSet<Vector2Int>();

        // 4 touching edges: Forward, Back, Left, Right
        private static readonly Vector2Int[] CardinalDirections =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        private float _spawnTimer;
        private Vector2Int _lastGridCell;
        private Vector3 _gridOrigin;
        private bool _isSpawning;

        public event Action OnNewTileStepped;

        public TileSpawnerController(SpawnerConfigSO config, IObjectPool<TileView> tileViewPool)
        {
            _config = config;
            _tileViewPool = tileViewPool;
        }

        public void StartSpawning(Vector3 playerStartPosition)
        {
            StopAndClearAll();
            _isSpawning = true;

            // Lock all tiles to Y = 0f right below the player
            _gridOrigin = new Vector3(playerStartPosition.x, 0f, playerStartPosition.z);
            _lastGridCell = Vector2Int.zero;

            // 1. Spawn the starting tile under the player (lasts full lifetime)
            SpawnSingleTile(_lastGridCell, _config.TileLifetime, isStartingTile: true);

            // 2. Set timer so the next tile spawns after a short 0.6s beat (1 by 1!)
            _spawnTimer = 0.6f;
        }

        public void StopAndClearAll()
        {
            _isSpawning = false;

            for (int i = _activeControllers.Count - 1; i >= 0; i--)
            {
                DeactivateTile(_activeControllers[i]);
            }
            _activeControllers.Clear();
            _occupiedGridCells.Clear();
            _tileViewPool.ClearAllActive();
        }

        public void Tick(float deltaTime)
        {
            if (!_isSpawning) return;

            // 1. Spawn strictly ONE tile per interval so they appear and vanish 1-by-1!
            _spawnTimer -= deltaTime;
            if (_spawnTimer <= 0f)
            {
                _spawnTimer = _config.SpawnInterval;
                SpawnNextTileInChain();
            }

            // 2. Tick all active tiles so each one counts down independently
            for (int i = _activeControllers.Count - 1; i >= 0; i--)
            {
                _activeControllers[i].Tick(deltaTime);
            }
        }

        private void SpawnNextTileInChain()
        {
            Vector2Int nextCell = GetUnoccupiedNeighborCell(_lastGridCell);
            SpawnSingleTile(nextCell, _config.TileLifetime, isStartingTile: false);
            _lastGridCell = nextCell;
        }

        private Vector2Int GetUnoccupiedNeighborCell(Vector2Int fromCell)
        {
            int startIndex = Random.Range(0, CardinalDirections.Length);

            for (int i = 0; i < CardinalDirections.Length; i++)
            {
                Vector2Int dir = CardinalDirections[(startIndex + i) % CardinalDirections.Length];
                Vector2Int candidate = fromCell + dir;

                if (!_occupiedGridCells.Contains(candidate))
                {
                    return candidate;
                }
            }

            return fromCell + CardinalDirections[startIndex] * 2;
        }

        private void SpawnSingleTile(Vector2Int gridCell, float lifetime, bool isStartingTile)
        {
            TileView view = _tileViewPool.Get();
            float tileSize = view.TileWidth;

            Vector3 worldPos = new Vector3(
                _gridOrigin.x + (gridCell.x * tileSize),
                _gridOrigin.y,
                _gridOrigin.z + (gridCell.y * tileSize)
            );

            view.SetPosition(worldPos);
            _occupiedGridCells.Add(gridCell);

            if (!_controllerLookup.TryGetValue(view, out TileController controller))
            {
                controller = new TileController(new TileModel(), view);
                _controllerLookup[view] = controller;
            }

            controller.OnTileExpired += HandleTileExpired;
            controller.OnValidTileStepped += HandleValidTileStepped;
            controller.Activate(lifetime, isStartingTile);

            _activeControllers.Add(controller);
        }

        private void HandleValidTileStepped()
        {
            OnNewTileStepped?.Invoke();
        }

        private void HandleTileExpired(TileController controller)
        {
            RemoveOccupiedCellForView(controller.TileView);
            DeactivateTile(controller);
            _activeControllers.Remove(controller);
        }

        private void RemoveOccupiedCellForView(TileView view)
        {
            float tileSize = view.TileWidth;
            Vector3 pos = view.transform.position;
            Vector2Int cell = new Vector2Int(
                Mathf.RoundToInt((pos.x - _gridOrigin.x) / tileSize),
                Mathf.RoundToInt((pos.z - _gridOrigin.z) / tileSize)
            );
            _occupiedGridCells.Remove(cell);
        }

        private void DeactivateTile(TileController controller)
        {
            controller.Dispose();
            _tileViewPool.ReturnToPool(controller.TileView);
        }

        public void Dispose()
        {
            StopAndClearAll();
            OnNewTileStepped = null;
        }
    }
}
using TileGame.Configs;
using TileGame.Player;
using TileGame.Services;
using TileGame.Tiles;
using TileGame.UI;
using UnityEngine;

namespace TileGame.Core
{
    /// <summary>
    /// The Composition Root (Entry Point) of the game.
    /// Replaces Singletons by manually instantiating and injecting all dependencies via constructors.
    /// Tomorrow, when migrating to Zenject, this class is easily converted into a MonoInstaller!
    /// </summary>
    public class GameBootstrapper : MonoBehaviour
    {
        [Header("ScriptableObject Configs")]
        [SerializeField] private GameConfigSO _gameConfig;
        [SerializeField] private PlayerConfigSO _playerConfig;
        [SerializeField] private SpawnerConfigSO _spawnerConfig;

        [Header("Scene Views & Prefabs")]
        [SerializeField] private PlayerView _playerView;
        [SerializeField] private TileView _tilePrefab;
        [SerializeField] private Transform _tilePoolParent;
        [SerializeField] private UIRootView _uiRootView;

        // Services
        private IInputService _inputService;
        private IPauseService _pauseService;
        private IObjectPool<TileView> _tilePool;

        // Models
        private GameStateModel _gameStateModel;
        private PlayerModel _playerModel;

        // Controllers
        private PlayerController _playerController;
        private TileSpawnerController _tileSpawnerController;
        private UIController _uiController;

        private void Start()
        {
            // 1. Initialize Core Services
            _inputService = new UnityInputService();
            _pauseService = new PauseService();
            _tilePool = new GenericObjectPool<TileView>(
                _tilePrefab, 
                _spawnerConfig.InitialPoolSize, 
                _tilePoolParent
            );

            // 2. Initialize Pure C# Models
            _gameStateModel = new GameStateModel(_gameConfig);
            _playerModel = new PlayerModel(_playerConfig);

            // 3. Initialize Pure C# Controllers (Constructor Dependency Injection!)
            _playerController = new PlayerController(_playerModel, _playerView, _inputService);
            _tileSpawnerController = new TileSpawnerController(_spawnerConfig, _tilePool);
            _uiController = new UIController(_gameStateModel, _uiRootView);

            // 4. Wire High-Level Game Flow Events
            SubscribeEvents();

            // 5. Start in Main Menu with game paused
            _pauseService.SetPaused(true);
            _gameStateModel.SetState(GameState.MainMenu);
        }

        private void Update()
        {
            // Requirement #3: If paused or not in Playing state, do NOT tick gameplay controllers!
            if (_pauseService.IsPaused || _gameStateModel.CurrentState != GameState.Playing)
                return;

            float dt = Time.deltaTime;
            _playerController.Tick(dt);
            _tileSpawnerController.Tick(dt);
        }

        private void SubscribeEvents()
        {
            // Input Pause Key (ESC / P)
            _inputService.OnPausePressed += TogglePause;

            // UI Button Requests
            _uiController.OnStartGameRequested += StartNewGame;
            _uiController.OnPauseGameRequested += TogglePause;
            _uiController.OnResumeGameRequested += ResumeGame;
            _uiController.OnRestartGameRequested += StartNewGame;

            // Gameplay Events
            _tileSpawnerController.OnNewTileStepped += HandleTileStepped;
            _playerController.OnPlayerFell += HandlePlayerFell;
            _gameStateModel.OnStateChanged += HandleGameStateChanged;
        }

        private void StartNewGame()
        {
            _pauseService.SetPaused(false);
            _gameStateModel.ResetScore();
            _gameStateModel.SetState(GameState.Playing);

            _playerController.SpawnPlayer();
            _tileSpawnerController.StartSpawning(_playerConfig.StartPosition);
        }

        private void TogglePause()
        {
            if (_gameStateModel.CurrentState == GameState.Playing)
            {
                _pauseService.SetPaused(true);
                _gameStateModel.SetState(GameState.Paused);
            }
            else if (_gameStateModel.CurrentState == GameState.Paused)
            {
                ResumeGame();
            }
        }

        private void ResumeGame()
        {
            if (_gameStateModel.CurrentState != GameState.Paused) return;

            _pauseService.SetPaused(false);
            _gameStateModel.SetState(GameState.Playing);
        }

        private void HandleTileStepped()
        {
            _gameStateModel.IncrementScore();
        }

        private void HandlePlayerFell()
        {
            if (_gameStateModel.CurrentState != GameState.Playing) return;
            _gameStateModel.SetState(GameState.GameOver);
        }

        private void HandleGameStateChanged(GameState state)
        {
            if (state == GameState.Victory || state == GameState.GameOver)
            {
                _playerController.SetActive(false);
                _tileSpawnerController.StopAndClearAll();
                _pauseService.SetPaused(true);
            }
        }

        private void OnDestroy()
        {
            // Clean up all subscriptions and native InputActions
            if (_inputService != null)
            {
                _inputService.OnPausePressed -= TogglePause;
                _inputService.Dispose();
            }

            if (_uiController != null)
            {
                _uiController.OnStartGameRequested -= StartNewGame;
                _uiController.OnPauseGameRequested -= TogglePause;
                _uiController.OnResumeGameRequested -= ResumeGame;
                _uiController.OnRestartGameRequested -= StartNewGame;
                _uiController.Dispose();
            }

            if (_tileSpawnerController != null)
            {
                _tileSpawnerController.OnNewTileStepped -= HandleTileStepped;
                _tileSpawnerController.Dispose();
            }

            if (_playerController != null)
            {
                _playerController.OnPlayerFell -= HandlePlayerFell;
                _playerController.Dispose();
            }

            if (_gameStateModel != null)
            {
                _gameStateModel.OnStateChanged -= HandleGameStateChanged;
            }

            // Ensure timeScale is restored if scene reloads
            Time.timeScale = 1f;
        }
    }
}
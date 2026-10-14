using System;
using UnityEngine;

namespace TileGame.Core
{
    // interface for our MVC Controller
    public interface IModel { }
    public interface IView { }

    // Allows pure C# controllers to run Update() without being MonoBehaviours!
    public interface ITickable
    {
        void Tick(float deltaTime);
    }

    // things which we can pool!
    public interface IPoolable
    {
        void OnSpawn();
        void OnDespawn();
    }

    // Pool Contract
    public interface IObjectPool<T> where T : Component, IPoolable
    {
        T Get();
        void ReturnToPool(T item);
        void ClearAllActive();
    }

    // Input Contract
    public interface IInputService : IDisposable
    {
        Vector2 MoveInput { get; }
        event Action OnPausePressed;
    }

    // Pause Contract
    public interface IPauseService
    {
        bool IsPaused { get; }
        void SetPaused(bool isPaused);
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace TileGame.Core
{
    /// <summary>
    /// High-performance Generic Object Pool for any Unity Component that implements IPoolable.
    /// Pre-warms objects at start to ensure zero CPU spikes or Garbage Collection during gameplay.
    /// </summary>
    public class GenericObjectPool<T> : IObjectPool<T> where T : Component, IPoolable
    {
        private readonly T _prefab;
        private readonly Transform _poolParent;
        private readonly Queue<T> _inactiveQueue = new Queue<T>();
        private readonly HashSet<T> _activeItems = new HashSet<T>();

        public GenericObjectPool(T prefab, int initialSize, Transform poolParent = null)
        {
            _prefab = prefab;
            _poolParent = poolParent;

            // Pre-warm the pool at startup so we don't Instantiate during gameplay
            for (int i = 0; i < initialSize; i++)
            {
                T newItem = CreateNewInstance();
                newItem.gameObject.SetActive(false);
                _inactiveQueue.Enqueue(newItem);
            }
        }

        public T Get()
        {
            // Grab a sleeping object from the queue, or create one if the pool ran dry
            T item = _inactiveQueue.Count > 0 ? _inactiveQueue.Dequeue() : CreateNewInstance();

            _activeItems.Add(item);
            item.gameObject.SetActive(true);
            item.OnSpawn(); // Tell the object to reset itself!

            return item;
        }

        public void ReturnToPool(T item)
        {
            // Safety check: ignore if already in pool (prevents double-return bugs!)
            if (!_activeItems.Remove(item)) return;

            item.OnDespawn();
            item.gameObject.SetActive(false);
            _inactiveQueue.Enqueue(item);
        }

        public void ClearAllActive()
        {
            // Copy to small temp list to safely iterate while returning all active tiles on Game Restart
            List<T> currentActive = new List<T>(_activeItems);
            foreach (T item in currentActive)
            {
                ReturnToPool(item);
            }
        }

        private T CreateNewInstance()
        {
            return Object.Instantiate(_prefab, _poolParent);
        }
    }
}
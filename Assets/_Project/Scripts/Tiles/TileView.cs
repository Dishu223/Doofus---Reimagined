using System;
using TileGame.Core;
using TileGame.Player;
using UnityEngine;

namespace TileGame.Tiles
{
    [RequireComponent(typeof(Collider))]
    public class TileView : MonoBehaviour, IView, IPoolable
    {
        [SerializeField] private Renderer _meshRenderer;
        [SerializeField] private Color _defaultColor = new Color(0.2f, 0.6f, 1f);     // Cool Blue
        [SerializeField] private Color _steppedColor = new Color(0.2f, 0.85f, 0.4f);  // Victory Green
        [SerializeField] private Color _warningColor = new Color(1f, 0.3f, 0.3f);     // Danger Red (last 1.5s)

        private MaterialPropertyBlock _propBlock;
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor"); // URP standard shader ID
        private static readonly int LegacyColorId = Shader.PropertyToID("_Color");   // Built-in fallback

        public event Action<TileView> OnPlayerLanded;
        // Reads the exact X width of the tile so adjacent tiles touch edge-to-edge!
        public float TileWidth => transform.localScale.x;

        private void Awake()
        {
            _propBlock = new MaterialPropertyBlock();
            if (_meshRenderer == null)
                _meshRenderer = GetComponentInChildren<Renderer>();
        }

        public void OnSpawn()
        {
            SetColor(_defaultColor);
        }

        public void OnDespawn()
        {
            OnPlayerLanded = null;
        }

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void MarkSteppedVisual()
        {
            SetColor(_steppedColor);
        }

        public void ShowExpiringVisual()
        {
            SetColor(_warningColor);
        }

        private void SetColor(Color color)
        {
            if (_meshRenderer == null) return;
            _meshRenderer.GetPropertyBlock(_propBlock);
            _propBlock.SetColor(BaseColorId, color);
            _propBlock.SetColor(LegacyColorId, color);
            _meshRenderer.SetPropertyBlock(_propBlock);
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Check if the colliding object is the PlayerView
            if (collision.gameObject.TryGetComponent<PlayerView>(out _))
            {
                OnPlayerLanded?.Invoke(this);
            }
        }
    }
}
using System;
using TileGame.Core;

namespace TileGame.Tiles
{
    public class TileController : BaseController<TileModel, TileView>
    {
        public event Action<TileController> OnTileExpired;
        public event Action OnValidTileStepped;

        private bool _warningShown;

        public TileView TileView => View;

        public TileController(TileModel model, TileView view) : base(model, view) { }

        public void Activate(float lifetime, bool isStartingTile = false)
        {
            Model.Reset(lifetime, isStartingTile);
            _warningShown = false;

            if (isStartingTile)
                View.MarkSteppedVisual();

            View.OnPlayerLanded += HandlePlayerLanded;
        }

        public void Tick(float deltaTime)
        {
            Model.RemainingTime -= deltaTime;

            // Turn red when less than 1.2 seconds left so the player knows it's vanishing!
            if (!_warningShown && Model.RemainingTime <= 1.2f)
            {
                _warningShown = true;
                View.ShowExpiringVisual();
            }

            if (Model.RemainingTime <= 0f)
            {
                OnTileExpired?.Invoke(this);
            }
        }

        private void HandlePlayerLanded(TileView view)
        {
            if (Model.HasBeenSteppedOn) return;

            Model.HasBeenSteppedOn = true;
            View.MarkSteppedVisual();
            OnValidTileStepped?.Invoke();
        }

        public override void Dispose()
        {
            View.OnPlayerLanded -= HandlePlayerLanded;
            OnTileExpired = null;
            OnValidTileStepped = null;
        }
    }
}
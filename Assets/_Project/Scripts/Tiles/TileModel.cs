using TileGame.Core;

namespace TileGame.Tiles
{
    public class TileModel : IModel
    {
        public float TotalLifetime { get; private set; }
        public float RemainingTime { get; set; }
        public bool HasBeenSteppedOn { get; set; }
        public bool IsFirstStartingTile { get; private set; }

        public void Reset(float lifetime, bool isStartingTile = false)
        {
            TotalLifetime = lifetime;
            RemainingTime = lifetime;
            HasBeenSteppedOn = isStartingTile; // Starting platform doesn't give a free point
            IsFirstStartingTile = isStartingTile;
        }
    }
}
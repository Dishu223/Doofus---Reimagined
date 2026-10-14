using System;

namespace TileGame.Core
{
    // Generic MVC Base Controller.
    // Enforces that every Controller connects a pure C# Model (TModel) and a View (TView).
    public abstract class BaseController<TModel, TView> : IDisposable
        where TModel : IModel
        where TView : IView
    {
        protected readonly TModel Model;
        protected readonly TView View;

        protected BaseController(TModel model, TView view)
        {
            Model = model;
            View = view;
        }

        // Clean up event subscriptions here so we NEVER get memory leaks!
        public abstract void Dispose();
    }
}
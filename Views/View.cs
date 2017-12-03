using UnityEngine;
using Becerra.MVP.Model;
using Becerra.MVP.Pools;

namespace Becerra.MVP.Views
{
    /// <summary>
    /// World object visually representing a set of data from the game.
    /// </summary>
    /// <typeparam name="T">Type of data to represent.</typeparam>
    public abstract class View<T> : MonoBehaviour, IUpdatableView<T> where T: class, IModel
    {
        /// <summary>
        /// Generic set of data this view represents.
        /// </summary>
        public IModel BaseModel { get; private set; }
        
        /// <summary>
        /// Specific set of data this view represents.
        /// </summary>
        public T Model { get; private set; }
        
        /// <summary>
        /// Pool that spawned this view.
        /// </summary>
        public IViewDisposer ViewDisposer { get; set; }
        
        /// <summary>
        /// Scene object used to visuallize the data in the world.
        /// </summary>
        public GameObject SceneObject { get { return gameObject; } }

        /// <summary>Updates the visuals of this view to represent the given model</summary>
        /// <param name="model">model</param>
        public void Refresh(T model)
        {
            if (model == null) return;

            Model = model;
            BaseModel = model;
            RefreshImplementation(model);
        }

        /// <summary>Updates the visuals of this view to represent the given model</summary>
        /// <param name="model">model</param>
        public void Refresh(IModel model)
        {
            Refresh(model as T);
        }

        /// <summary>Resets the state of the view so it can be reused by the pool later on with no problem.</summary>
        public void Clean()
        {
            CleanImplementation();
            Model = null;
            BaseModel = null;
        }

        /// <summary>
        /// Additional logic to clean the view.
        /// </summary>
        protected abstract void CleanImplementation();
        
        /// <summary>
        /// Additional logic to refresh the view so it represents the data being given.
        /// </summary>
        /// <param name="model"></param>
        protected abstract void RefreshImplementation(T model);

        /// <summary>
        /// Gets rid of this view. returning it to the pool it spawned from.
        /// </summary>
        public void Dispose()
        {
            if (ViewDisposer == null) return;

            ViewDisposer.Dispose(this);
        }
    }
}

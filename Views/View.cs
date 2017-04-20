using UnityEngine;
using Becerra.MVP.Model;

namespace Becerra.MVP.Views
{
    public abstract class View<T> : MonoBehaviour, IUpdatableView<T> where T: class, IModel
    {
        public IModel BaseModel { get; private set; }
        public T Model { get; private set; }
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

        protected abstract void CleanImplementation();
        protected abstract void RefreshImplementation(T model);
    }
}

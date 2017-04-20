using UnityEngine;
using Becerra.MVP.Model;

namespace Becerra.MVP.Views
{
    public abstract class View<T> : MonoBehaviour, IUpdatableView<T> where T: class, IModel
    {
        public IModel BaseModel { get; private set; }
        public T Model { get; private set; }
        public GameObject SceneObject { get { return gameObject; } }

        public void Refresh(T model)
        {
            if (model == null) return;

            Model = model;
            BaseModel = model;
            RefreshImplementation(model);
        }

        public void Refresh(IModel model)
        {
            Refresh(model as T);
        }

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

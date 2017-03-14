using UnityEngine;
using Becerra.MVP.Model;

namespace Becerra.MVP.Views
{
    public abstract class View<T> : MonoBehaviour, IUpdatableView<T> where T: class, IModel
    {
        public IModel BaseModel { get; private set; }
        public T Model { get; private set; }
        public GameObject SceneObject { get; protected set; }

        public void Refresh(T model)
        {
            Model = model;
            BaseModel = model;
            RefreshImplementation(model);
        }

        public void Refresh(IModel model)
        {
            Refresh(model as T);
        }

        public abstract void Clean();
        protected abstract void RefreshImplementation(T model);
    }
}

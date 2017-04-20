using UnityEngine;
using Becerra.MVP.Model;

namespace Becerra.MVP.Views
{
    public interface IUpdatableView
    {
        GameObject SceneObject { get; }
        IModel BaseModel { get; }

        void Refresh(IModel model);
        void Clean();
    }

    public interface IUpdatableView<T> : IUpdatableView where T: class, IModel
    {
        T Model { get; }
        void Refresh(T model);
    }
}

using Becerra.MVP.Model;
using Becerra.MVP.Views;

namespace Becerra.MVP.Pools
{
    public interface IPool
    {
        IUpdatableView BasePrefab { get; }

        void Initialize(IUpdatableView prefab, int initialCount);
        IUpdatableView Provide(IModel model);
        bool Free(IUpdatableView view);
        bool Free(IModel model);
        bool Free(string id);
        IUpdatableView Find(string id);
        IUpdatableView Find(IModel model);
        void Clean();
    }

    public interface IPool<T> : IPool where T: class, IModel
    {
        IUpdatableView<T> Prefab { get; }
        View<T> ViewPrefab { get; }

        void Initialize(View<T> prefab, int initialCount);
        View<T> Provide(T model);
        bool Free(IUpdatableView<T> view);
        bool Free(T model);
        View<T> Find(T model);
    }
}

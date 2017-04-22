using Becerra.MVP.Model;
using Becerra.MVP.Views;
using System.Collections.Generic;
using UnityEngine;

namespace Becerra.MVP.Pools
{
    public interface IPool
    {
        /// <summary>Parent for pooled elements. Must be set before calling Initialize.</summary>
        Transform Container { get; set; }

        /// <summary>Prefab used to instantiate new pool elements.</summary>
        IUpdatableView BasePrefab { get; }

        /// <summary>Initializes the pool. Must be used before being used.</summary>
        /// <param name="prefab">prefab</param>
        /// <param name="initialCount">initialCount</param>
        void Initialize(IUpdatableView prefab, int initialCount);

        /// <summary>Provides a view representing the model provided.</summary>
        /// <param name="model">model</param>
        /// <returns>IUpdatableView</returns>
        IUpdatableView Provide(IModel model);

        /// <summary>Frees the given view.</summary>
        /// <param name="view">view</param>
        /// <returns>bool</returns>
        bool Free(IUpdatableView view);

        /// <summary>Frees the view associated with the given model.</summary>
        /// <param name="model">model</param>
        /// <returns>bool</returns>
        bool Free(IModel model);

        /// <summary>Frees the view associated with a model with the same id.</summary>
        /// <param name="id">id</param>
        /// <returns>bool</returns>
        bool Free(string id);

        /// <summary>Finds a used view associated with a model with the given id.</summary>
        /// <param name="id">id</param>
        /// <returns>IUpdatableView</returns>
        IUpdatableView Find(string id);

        /// <summary>Finds a used view associated with the provided model.</summary>
        /// <param name="model">model</param>
        /// <returns>IUpdatableView</returns>
        IUpdatableView Find(IModel model);

        /// <summary>Cleans the pool so it's at the initial state.</summary>
        void Clean();
    }

    public interface IPool<T> : IPool, IEnumerable<IUpdatableView<T>> where T: class, IModel
    {
        /// <summary>Prefab used to instantiate new pool elements.</summary>
        IUpdatableView<T> Prefab { get; }

        /// <summary>Prefab used to instantite new pool elements.</summary>
        View<T> ViewPrefab { get; }

        /// <summary>Provides a new view representing the model given.</summary>
        /// <param name="model">model</param>
        /// <returns>View<T></returns>
        View<T> Provide(T model);

        /// <summary>Frees the view given.</summary>
        /// <param name="view">view</param>
        /// <returns>bool</returns>
        bool Free(IUpdatableView<T> view);

        /// <summary>Frees a used view associated with the given model</summary>
        /// <param name="model">model</param>
        /// <returns>bool</returns>
        bool Free(T model);

        /// <summary>Find a used view associated with the given model.</summary>
        /// <param name="model">model</param>
        /// <returns>View<T></returns>
        View<T> Find(T model);
    }
}

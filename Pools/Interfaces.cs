using System;
using Becerra.MVP.Model;
using Becerra.MVP.Views;
using System.Collections.Generic;
using UnityEngine;

namespace Becerra.MVP.Pools
{
    /// <summary>
    /// Generic pool for any monobehaviour.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPool<T> : IEnumerable<T>, IDisposable where T : MonoBehaviour
    {
        /// <summary>Parent for pooled elements. Must be set before calling Initialize.</summary>
        Transform Container { get; set; }
        
        /// <summary>
        /// Prefab used to create new elements in the pool.
        /// </summary>
        T Prefab { get; }
        
        /// <summary>
        /// Provides a new pool element to be used.
        /// </summary>
        /// <returns></returns>
        T Provide();
        
        /// <summary>
        /// Frees the scene object back into the pool so it can be used again later.
        /// </summary>
        /// <param name="sceneObject"></param>
        /// <returns></returns>
        bool Free(T sceneObject);
    }

    /// <summary>
    /// Specialliced pool for views.
    /// </summary>
    /// <typeparam name="T">Model</typeparam>
    /// <typeparam name="TR">View type used to represent the model.</typeparam>
    public interface IViewPool<T, TR> : IEnumerable<TR> where T: class, IModel where TR: IUpdatableView<T>
    {
        /// <summary>Prefab used to instantiate new pool elements.</summary>
        IUpdatableView BaseViewPrefab { get; }

        /// <summary>Prefab used to instantite new pool elements.</summary>
        View<T> ViewPrefab { get; }
        
        /// <summary>Provides a view representing the model provided.</summary>
        /// <param name="model">model</param>
        /// <returns>IUpdatableView</returns>
        IUpdatableView Provide(IModel model);

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

        /// <summary>Find a used view associated with the given model.</summary>
        /// <param name="model">model</param>
        /// <returns>View<T></returns>
        View<T> Find(T model);
    }
}

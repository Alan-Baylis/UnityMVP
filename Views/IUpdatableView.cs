using System;
using UnityEngine;
using Becerra.MVP.Model;
using Becerra.MVP.Pools;

namespace Becerra.MVP.Views
{
    /// <summary>
    /// World object that represents a chunk of data from the game.
    /// </summary>
    public interface IUpdatableView : IDisposable
    {
        /// <summary>
        /// Object in the scene used by this view.
        /// </summary>
        GameObject SceneObject { get; }
        
        /// <summary>
        /// Chunk of data associated with this view.
        /// </summary>
        IModel BaseModel { get; }

        /// <summary>
        /// Refreshes the view to represent the model given.
        /// Be careful to subscribe to delegates/message buses here if you are calling Refresh more than once without
        /// calling Clean(). Either check subcriptions or manually call Clean before Refresh.
        /// </summary>
        /// <param name="model"></param>
        void Refresh(IModel model);
        
        /// <summary>
        /// Cleans the view from any previous usage.
        /// Very important to unsubscrine to delegates/message buses.
        /// </summary>
        void Clean();
    }

    /// <summary>
    /// World object that represents a chunk of data from the game of a certain type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IUpdatableView<T> : IUpdatableView where T: class, IModel
    {
        /// <summary>
        /// Chunk of data associated with the view.
        /// </summary>
        T Model { get; }
        
        /// <summary>
        /// Pool that spawned this view.
        /// </summary>
        IViewPool<T, IUpdatableView<T>> SourcePool { get; set; }
        
        /// <summary>
        /// Refreshes the view to represent the model given.
        /// Be careful to subscribe to delegates/message buses here if you are calling Refresh more than once without
        /// calling Clean(). Either check subcriptions or manually call Clean before Refresh.
        /// </summary>
        /// <param name="model"></param>
        void Refresh(T model);
    }
}

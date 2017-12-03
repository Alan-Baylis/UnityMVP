using UnityEngine;
using Becerra.MVP.Model;
using Becerra.MVP.Views;
using System.Collections.Generic;

namespace Becerra.MVP.Pools
{
    /// <summary>
    /// Basic pool that will have an initial amount of elements and will expand as needed.
    /// Freed elements will be reused as soon as a new element is required.
    /// </summary>
    public class SimplePool<T> : IPool<T> where T: class, IModel
    {
        /// <summary>
        /// Internal representation of a pool element.
        /// </summary>
        protected class Node
        {
            public View<T> View;
            public bool IsUsed;
        }

        /// <summary>
        /// Where the pool elements are instantiated when created or placed when freed.
        /// </summary>
        /// <value>The pool parent.</value>
        public Transform Container { get; set; }

        /// <summary>
        /// Prefab used to create new instances of pool elements.
        /// </summary>
        /// <value>The base prefab.</value>
        public IUpdatableView BasePrefab { get { return ViewPrefab; } private set { ViewPrefab = value as View<T>; } }

        /// <summary>
        /// Prefab used to create new instances of pool elements.
        /// </summary>
        /// <value>The prefab.</value>
        public IUpdatableView<T> Prefab { get { return ViewPrefab; } private set { ViewPrefab = value as View<T>; } }

        /// <summary>
        /// Prefab used to create new instances of pool elements.
        /// </summary>
        /// <value>The view prefab.</value>
        public View<T> ViewPrefab { get; private set; }

        /// <summary>
        /// List of pool elements available and used.
        /// </summary>
        protected IList<Node> Nodes;

        /// <summary>
        /// Views currently being used.
        /// This is a helper list for easier iteration over the pool.
        /// </summary>
        protected IList<IUpdatableView<T>> UsedViews;

        /// <summary>
        /// Prepares the pool to be used. This must be called before using any of the other methods.
        /// </summary>
        /// <param name="prefab">Prefab.</param>
        /// <param name="initialCount">Initial count.</param>
        public void Initialize(IUpdatableView prefab, int initialCount)
        {
            ViewPrefab = prefab as View<T>;
            Prefab = prefab as IUpdatableView<T>;
            BasePrefab = prefab;

            if (ViewPrefab == null || Prefab == null || BasePrefab == null)
            {
                throw new System.InvalidCastException("View " + prefab + " is not of the required type for pool for type" + typeof(T));
            }

            Nodes = new List<Node>();
            UsedViews = new List<IUpdatableView<T>>();

            for (int i = 0; i < initialCount; i++)
            {
                Expand(ViewPrefab);
            }
        }

        /// <summary>
        /// Prepares the pool to be used. This must be called before using any of the other methods.
        /// </summary>
        /// <param name="prefab">Prefab.</param>
        /// <param name="initialCount">Initial count.</param>
        /// <param name="container">Object in the scene that will holde the pooled objects.</param>
        public void Initialize(View<T> prefab, int initialCount, Transform container = null)
        {
            Container = container;
            ViewPrefab = prefab;
            Initialize(prefab as IUpdatableView<T>, initialCount);
        }

        /// <summary>
        /// Prepares the pool to be used. This must be called before using any of the other methods.
        /// </summary>
        /// <param name="prefab">prefab</param>
        /// <param name="initialCount">initialCount</param>
        /// <param name="container"></param>
        public void Initialize(IUpdatableView<T> prefab, int initialCount, Transform container = null)
        {
            Container = container;
            Prefab = prefab;
            Initialize(prefab, initialCount);
        }
        
        /// <summary>
        /// How to dispose the pool.
        /// This will free al views (cleaning them first) and the empty all collections.
        /// Views are cleaned to ensure there is nothing left behind (like delegates subscriptions etc).
        /// </summary>
        public void Dispose()
        {
            Clean();
        }

        /// <summary>
        /// Provides a new view representing the data model provided.
        /// If there is no available view, a new one will be instantiated.
        /// </summary>
        /// <param name="model">Model.</param>
        public virtual View<T> Provide(T model)
        {
            var node = FindAvailableView();

            if (node == null)
            {
                node = Expand(ViewPrefab);
            }

            node.View.Refresh(model);
            node.View.gameObject.SetActive(true);
            node.IsUsed = true;
            node.View.name =  model.Id + " [ View for " + typeof(T) + " ]";

            UsedViews.Add(node.View);

            return node.View;
        }

        /// <summary>
        /// Provides a new view representing the data model provided.
        /// If there is no available view, a new one will be instantiated.
        /// </summary>
        /// <param name="model">Model.</param>
        public IUpdatableView Provide(IModel model)
        {
            return Provide(model as T);
        }

        /// <summary>
        /// Finds a view with an associated model with the same id as the provided one.
        /// </summary>
        /// <param name="id">Identifier.</param>
        public IUpdatableView Find(string id)
        {
            foreach (var node in Nodes)
            {
                if (node.IsUsed == false) continue;

                if (node.View.BaseModel.Id == id) return node.View;
            }

            return null;
        }

        /// <summary>
        /// Finds the view associated with the given model.
        /// </summary>
        /// <param name="model">Model.</param>
        public IUpdatableView Find(IModel model)
        {
            return Find(model.Id);
        }

        /// <summary>
        /// Finds the view associated with the given model.
        /// </summary>
        /// <param name="model">Model.</param>
        public View<T> Find(T model)
        {
            return Find(model.Id) as View<T>;
        }

        /// <summary>
        /// Frees the view associated with the given model.
        /// </summary>
        /// <param name="model">Model.</param>
        public bool Free(IModel model)
        {
            return Free(model.Id);
        }

        /// <summary>
        /// Frees the view associated with the given model.
        /// </summary>
        /// <param name="model">Model.</param>
        public bool Free(T model)
        {
            return Free(model.Id);
        }

        /// <summary>
        /// Frees the view associated with the given model.
        /// </summary>
        /// <param name="view">View.</param>
        public bool Free(IUpdatableView view)
        {
            return Free(view.BaseModel.Id);
        }

        /// <summary>
        /// Frees the view associated with the given model.
        /// </summary>
        /// <param name="view">View.</param>
        public bool Free(IUpdatableView<T> view)
        {
            return Free(view.BaseModel.Id);
        }

        /// <summary>
        /// Frees the view associated with the given model.
        /// </summary>
        /// <param name="view">View.</param>
        public bool Free(View<T> view)
        {
            return Free(view.BaseModel.Id);
        }

        /// <summary>
        /// Frees the view associated with a model with the given id.
        /// </summary>
        /// <param name="id">Identifier.</param>
        public virtual bool Free(string id)
        {
            var node = FindNode(id);

            if (node == null) return false;

            node.View.Clean();
            node.View.SceneObject.transform.SetParent(Container);
            node.View.SceneObject.SetActive(false);
            node.IsUsed = false;
            node.View.name = "--- [ View for " + typeof(T) + " ]";

            UsedViews.Remove(node.View);

            return true;
        }

        /// <summary>
        /// Frees all views and destroys all instances.
        /// </summary>
        public void Clean()
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                Nodes[i].View.Clean();
                GameObject.Destroy(Nodes[i].View.SceneObject);
            }

            Nodes.Clear();
            UsedViews.Clear();
        }

        /// <summary>
        /// Searchs for the first non already used view.
        /// </summary>
        /// <returns></returns>
        private Node FindAvailableView()
        {
            foreach (var node in Nodes)
            {
                if (node.IsUsed == false) return node;
            }

            return null;
        }

        /// <summary>
        /// Finds a node associated with the given model.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private Node FindNode(T model)
        {
            foreach (var node in Nodes)
            {
                if (node.IsUsed)
                {
                    if (node.View.Model == model) return node;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds a node associated with the given model.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private Node FindNode(IModel model)
        {
            foreach (var node in Nodes)
            {
                if (node.IsUsed)
                {
                    if (node.View.Model == model) return node;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds a node using the given view.
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        private Node FindNode(IUpdatableView view)
        {
            foreach (var node in Nodes)
            {
                if (node.IsUsed)
                {
                    if (node.View as IUpdatableView == view) return node;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds a node using the given view.
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        private Node FindNode(IUpdatableView<T> view)
        {
            foreach (var node in Nodes)
            {
                if (node.IsUsed)
                {
                    if (node.View as IUpdatableView == view) return node;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds a node using the given view.
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        private Node FindNode(View<T> view)
        {
            foreach (var node in Nodes)
            {
                if (node.IsUsed)
                {
                    if (node.View == view) return node;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds a node associated with a model with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private Node FindNode(string id)
        {
            foreach (var node in Nodes)
            {
                if (node.IsUsed == false) continue;

                if (node.View.BaseModel.Id == id) return node;
            }

            return null;
        }

        /// <summary>
        /// Creates a new view at the end of the pool.
        /// This is called automatically if there is not view available when providing one.
        /// Override this method if you want to create the objects in another way (like using Zenject pools/factories)
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        protected virtual Node Expand(View<T> prefab)
        {
            var view = GameObject.Instantiate(prefab, Container);

            view.SourcePool = this;
            view.gameObject.SetActive(false);
            view.transform.SetParent(Container);

            var node = new Node
            {
                View = view,
                IsUsed = false
            };

            node.View.name = "--- [ View for " + typeof(T) + " ]";

            Nodes.Add(node);

            return node;
        }

        #region IEnumerator

        public IEnumerator<IUpdatableView<T>> GetEnumerator()
        {
            return UsedViews.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return UsedViews.GetEnumerator();
        }

        #endregion
    }
}

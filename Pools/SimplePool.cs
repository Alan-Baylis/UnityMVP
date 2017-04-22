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
        class Node
        {
            public View<T> view;
            public bool isUsed;
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
        /// List of pool elements.
        /// </summary>
        private IList<Node> _nodes;

        /// <summary>
        /// Views currently being used.
        /// </summary>
        private IList<IUpdatableView<T>> _usedViews;

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

            _nodes = new List<Node>();
            _usedViews = new List<IUpdatableView<T>>();

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
        public void Initialize(View<T> prefab, int initialCount)
        {
            ViewPrefab = prefab;
            Initialize(prefab as IUpdatableView<T>, initialCount);
        }

        /// <summary>
        /// Prepares the pool to be used. This must be called before using any of the other methods.
        /// </summary>
        /// <param name="prefab">prefab</param>
        /// <param name="initialCount">initialCount</param>
        public void Initialize(IUpdatableView<T> prefab, int initialCount)
        {
            Prefab = prefab;
            Initialize(prefab, initialCount);
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

            node.view.Refresh(model);
            node.view.gameObject.SetActive(true);
            node.isUsed = true;
            node.view.name =  model.Id + " [ View for " + typeof(T) + " ]";

            _usedViews.Add(node.view);

            return node.view;
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
            for (int i = 0; i < _nodes.Count; i++)
            {
                if (_nodes[i].isUsed == false) continue;

                if (_nodes[i].view.BaseModel.Id == id) return _nodes[i].view;
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
        public bool Free(string id)
        {
            var node = FindNode(id);

            if (node == null) return false;

            node.view.Clean();
            node.view.SceneObject.transform.SetParent(Container);
            node.view.SceneObject.SetActive(false);
            node.isUsed = false;
            node.view.name = "--- [ View for " + typeof(T) + " ]";

            _usedViews.Remove(node.view);

            return true;
        }

        /// <summary>
        /// Frees all views and destroys all instances.
        /// </summary>
        public void Clean()
        {
            for (int i = 0; i < _nodes.Count; i++)
            {
                _nodes[i].view.Clean();
                GameObject.Destroy(_nodes[i].view.SceneObject);
            }

            _nodes.Clear();
            _usedViews.Clear();
        }

        private Node FindAvailableView()
        {
            for (int i = 0; i < _nodes.Count; i++)
            {
                if (_nodes[i].isUsed == false) return _nodes[i];
            }

            return null;
        }

        private Node FindNode(T model)
        {
            for (int i = 0; i < _nodes.Count; i++)
            {
                if (_nodes[i].isUsed)
                {
                    if (_nodes[i].view.Model == model) return _nodes[i];
                }
            }

            return null;
        }

        private Node FindNode(IModel model)
        {
            for (int i = 0; i < _nodes.Count; i++)
            {
                if (_nodes[i].isUsed)
                {
                    if (_nodes[i].view.Model == model) return _nodes[i];
                }
            }

            return null;
        }

        private Node FindNode(IUpdatableView view)
        {
            for (int i = 0; i < _nodes.Count; i++)
            {
                if (_nodes[i].isUsed)
                {
                    if (_nodes[i].view as IUpdatableView == view) return _nodes[i];
                }
            }

            return null;
        }

        private Node FindNode(IUpdatableView<T> view)
        {
            for (int i = 0; i < _nodes.Count; i++)
            {
                if (_nodes[i].isUsed)
                {
                    if (_nodes[i].view as IUpdatableView == view) return _nodes[i];
                }
            }

            return null;
        }

        private Node FindNode(View<T> view)
        {
            for (int i = 0; i < _nodes.Count; i++)
            {
                if (_nodes[i].isUsed)
                {
                    if (_nodes[i].view == view) return _nodes[i];
                }
            }

            return null;
        }

        private Node FindNode(string id)
        {
            for (int i = 0; i < _nodes.Count; i++)
            {
                if (_nodes[i].isUsed == false) continue;

                if (_nodes[i].view.BaseModel.Id == id) return _nodes[i];
            }

            return null;
        }

        private Node Expand(View<T> prefab)
        {
            var view = GameObject.Instantiate<View<T>>(prefab, Container);

            view.gameObject.SetActive(false);
            view.transform.SetParent(Container);

            Node node;

            node = new Node();
            node.view = view;
            node.isUsed = false;
            node.view.name = "--- [ View for " + typeof(T) + " ]";

            _nodes.Add(node);

            return node;
        }

        #region IEnumerator

        public IEnumerator<IUpdatableView<T>> GetEnumerator()
        {
            return _usedViews.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _usedViews.GetEnumerator();
        }

        #endregion
    }
}

using UnityEngine;
using Becerra.MVP.Model;
using Becerra.MVP.Views;
using System.Collections.Generic;

namespace Becerra.MVP.Pools
{
    public class SimplePool<T> : IPool<T> where T: class, IModel
    {
        class Node
        {
            public View<T> view;
            public bool isUsed;
        }

        public Transform PoolParent { get; set; }
        public IUpdatableView BasePrefab { get { return ViewPrefab; } private set { ViewPrefab = value as View<T>; } }
        public IUpdatableView<T> Prefab { get { return ViewPrefab; } private set { ViewPrefab = value as View<T>; } }
        public View<T> ViewPrefab { get; private set; }

        private IList<Node> _nodes;

        public void Initialize(IUpdatableView prefab, int initialCount)
        {
            _nodes = new List<Node>();

            for (int i = 0; i < initialCount; i++)
            {
                var node = Expand(ViewPrefab);

                _nodes.Add(node);
            }
        }

        public void Initialize(View<T> prefab, int initialCount)
        {
            ViewPrefab = prefab;
            Initialize(prefab as IUpdatableView, initialCount);
        }

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

            return node.view;
        }

        public IUpdatableView Provide(IModel model)
        {
            return Provide(model as T);
        }

        public bool Free(IModel model)
        {
            var node = FindView(model);

            if (node == null) return false;

            node.view.SceneObject.transform.SetParent(PoolParent);
            node.view.SceneObject.SetActive(false);
            node.isUsed = false;

            return true;
        }

        public bool Free(T model)
        {
            var node = FindView(model);

            if (node == null) return false;

            node.view.SceneObject.transform.SetParent(PoolParent);
            node.view.SceneObject.SetActive(false);
            node.isUsed = false;

            return true;
        }

        public bool Free(IUpdatableView view)
        {
            var node = FindView(view);

            if (node == null) return false;

            node.view.SceneObject.transform.SetParent(PoolParent);
            node.view.SceneObject.SetActive(false);
            node.isUsed = false;

            return true;
        }

        public bool Free(IUpdatableView<T> view)
        {
            var node = FindView(view);

            if (node == null) return false;

            node.view.SceneObject.transform.SetParent(PoolParent);
            node.view.SceneObject.SetActive(false);
            node.isUsed = false;

            return true;
        }

        public bool Free(View<T> view)
        {
            var node = FindView(view);

            if (node == null) return false;

            node.view.SceneObject.transform.SetParent(PoolParent);
            node.view.SceneObject.SetActive(false);
            node.isUsed = false;

            return true;
        }

        private Node FindAvailableView()
        {
            for (int i = 0; i < _nodes.Count; i++)
            {
                if (_nodes[i].isUsed == false) return _nodes[i];
            }

            return null;
        }

        private Node FindView(T model)
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

        private Node FindView(IModel model)
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

        private Node FindView(IUpdatableView view)
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

        private Node FindView(IUpdatableView<T> view)
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

        private Node FindView(View<T> view)
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

        private Node Expand(View<T> prefab)
        {
            var view = GameObject.Instantiate<View<T>>(prefab, PoolParent);

            view.gameObject.SetActive(false);
            view.transform.SetParent(PoolParent);

            Node node;

            node = new Node();
            node.view = view;
            node.isUsed = false;

            return node;
        }
    }
}

using UnityEngine;
using Becerra.MVP.Model;
using Becerra.MVP.Views;
using System.Collections.Generic;

namespace Becerra.MVP.Pools
{
    public class SimplePool<T> : IPool<T> where T: class, IModel
    {
        class Node<K> where K: class, IModel
        {
            public View<T> view;
            public bool isUsed;
        }

        public Transform PoolParent { get; private set; }
        public IUpdatableView BasePrefab { get { return ViewPrefab; } private set { ViewPrefab = value as View<T>; } }
        public IUpdatableView<T> Prefab { get { return ViewPrefab; } private set { ViewPrefab = value as View<T>; } }
        public View<T> ViewPrefab { get; private set; }

        private IList<Node<T>> _nodes;

        public void Initialize(IUpdatableView prefab, int initialCount)
        {
            _nodes = new List<Node<T>>();

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

        public View<T> Provide(T model)
        {
            var node = FindAvailableView();

            if (node == null)
            {
                node = Expand(ViewPrefab);
            }

            node.view.Refresh(model);
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

        private Node<T> FindAvailableView()
        {
            for (int i = 0; i < _nodes.Count; i++)
            {
                if (_nodes[i].isUsed == false) return _nodes[i];
            }

            return null;
        }

        private Node<T> FindView(T model)
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

        private Node<T> FindView(IModel model)
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

        private Node<T> FindView(IUpdatableView view)
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

        private Node<T> FindView(IUpdatableView<T> view)
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

        private Node<T> FindView(View<T> view)
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

        private Node<T> Expand(View<T> prefab)
        {
            var view = GameObject.Instantiate<View<T>>(prefab, PoolParent);

            view.gameObject.SetActive(false);
            view.transform.SetParent(PoolParent);

            Node<T> node;

            node = new Node<T>();
            node.view = view;
            node.isUsed = false;

            return node;
        }
    }
}

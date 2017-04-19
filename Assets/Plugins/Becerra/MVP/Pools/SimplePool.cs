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
                Expand(ViewPrefab);
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
            node.view.name =  model.Id + " [ View for " + typeof(T) + " ]";

            return node.view;
        }

        public IUpdatableView Provide(IModel model)
        {
            return Provide(model as T);
        }

        public IUpdatableView Find(string id)
        {
            for (int i = 0; i < _nodes.Count; i++)
            {
                if (_nodes[i].isUsed == false) continue;

                if (_nodes[i].view.BaseModel.Id == id) return _nodes[i].view;
            }

            return null;
        }

        public IUpdatableView Find(IModel model)
        {
            return Find(model.Id);
        }

        public View<T> Find(T model)
        {
            return Find(model.Id) as View<T>;
        }

        public bool Free(IModel model)
        {
            return Free(model.Id);
        }

        public bool Free(T model)
        {
            return Free(model.Id);
        }

        public bool Free(IUpdatableView view)
        {
            return Free(view.BaseModel.Id);
        }

        public bool Free(IUpdatableView<T> view)
        {
            return Free(view.BaseModel.Id);
        }

        public bool Free(View<T> view)
        {
            return Free(view.BaseModel.Id);
        }

        public bool Free(string id)
        {
            var node = FindNode(id);

            if (node == null) return false;

            node.view.SceneObject.transform.SetParent(PoolParent);
            node.view.SceneObject.SetActive(false);
            node.isUsed = false;
            node.view.name = "--- [ View for " + typeof(T) + " ]";

            return true;
        }

        public void Clean()
        {
            for (int i = 0; i < _nodes.Count; i++)
            {
                GameObject.Destroy(_nodes[i].view.SceneObject);
            }

            _nodes.Clear();
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
            var view = GameObject.Instantiate<View<T>>(prefab, PoolParent);

            view.gameObject.SetActive(false);
            view.transform.SetParent(PoolParent);

            Node node;

            node = new Node();
            node.view = view;
            node.isUsed = false;
            node.view.name = "--- [ View for " + typeof(T) + " ]";

            _nodes.Add(node);

            return node;
        }
    }
}

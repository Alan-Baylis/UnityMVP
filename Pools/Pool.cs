using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Becerra.MVP.Pools
{
    /// <summary>
    /// Pool for any kind of MonoBehaviour.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Pool<T> : IPool<T> where T: MonoBehaviour
    {
        /// <summary>
        /// Internal representation of a pool element.
        /// </summary>
        protected class Node
        {
            public T SceneObject;
            public bool IsUsed;
        }
        
        /// <inheritdoc />
        public Transform Container { get; set; }
        
        /// <inheritdoc />
        public T Prefab { get; private set; }
        
        protected IList<Node> Nodes { get; private set; }
        protected readonly IList<T> UsedElements;

        /// <summary>
        /// Default constructor. Creates the initial elements specified.
        /// </summary>
        /// <param name="prefab">Prefab used to create elements in the pool.</param>
        /// <param name="initialCount">Initial number of elements in the pool.</param>
        /// <param name="container">Where the pooled elements will be contained.</param>
        public Pool(T prefab, int initialCount, Transform container)
        {
            Prefab = prefab;
            Container = container;
            
            Nodes = new List<Node>(initialCount);
            UsedElements = new List<T>();

            for (int i = 0; i < initialCount; i++)
            {
                var node = CreateNode(prefab, container);
                
                Nodes.Add(node);
            }
        }

        /// <summary>
        /// Gets rid of the pool and its elements.
        /// </summary>
        public virtual void Dispose()
        {
            foreach (var node in Nodes)
            {
                GameObject.Destroy(node.SceneObject);
            }
            
            Nodes.Clear();
            UsedElements.Clear();
        }

        /// <inheritdoc />
        public T Provide()
        {
            var node = FindFirstNodeAvailable();

            if (node == null)
            {
                node = CreateNode(Prefab, Container);

                if (node == null) return null;
                
                Nodes.Add(node);
            }

            node.IsUsed = true;
            
            UsedElements.Add(node.SceneObject);
            
            return node.SceneObject;
        }

        /// <inheritdoc />
        public bool Dispose(T sceneObject)
        {
            var node = FindNode(sceneObject);

            if (node == null) return false;

            node.SceneObject.transform.SetParent(Container);
            node.SceneObject.transform.localPosition = Vector3.zero;
            node.SceneObject.gameObject.SetActive(false);
            node.IsUsed = false;
            
            UsedElements.Remove(sceneObject);

            return true;
        }

        /// <summary>
        /// Creates a new pool node containing info about a new pooled element.
        /// </summary>
        /// <param name="prefab">Prefab used to create the pooled element.</param>
        /// <param name="container">Where the pooled element is stored.</param>
        /// <returns>New node with info about the new pooled element.</returns>
        protected virtual Node CreateNode(T prefab, Transform container)
        {
            var obj = GameObject.Instantiate(prefab, container);

            if (obj == null) return null;
            
            obj.transform.SetParent(Container);
            obj.transform.localPosition = Vector3.zero;
            obj.gameObject.SetActive(false);

            var node = new Node
            {
                SceneObject = obj,
                IsUsed = false
            };

            return node;
        }

        /// <summary>
        /// Searchs for the first node with an available element.
        /// </summary>
        /// <returns></returns>
        protected Node FindFirstNodeAvailable()
        {
            foreach (var node in Nodes)
            {
                if (node.IsUsed == false) return node;
            }

            return null;
        }

        /// <summary>
        /// Searchs for a node in the pool associated with the object given.
        /// </summary>
        /// <param name="sceneObject"></param>
        /// <returns></returns>
        protected Node FindNode(T sceneObject)
        {
            foreach (var node in Nodes)
            {
                if (node.SceneObject == sceneObject) return node;
            }

            return null;
        }
        
        #region IEnumerable
        
        public IEnumerator<T> GetEnumerator()
        {
            return UsedElements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        #endregion
    }
}
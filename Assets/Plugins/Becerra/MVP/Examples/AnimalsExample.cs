using UnityEngine;
using Becerra.MVP.Model;
using Becerra.MVP.Views;
using Becerra.MVP.Pools;

namespace Becerra.MVP.Examples
{
    public class AnimalsExample : MonoBehaviour
    {
        public AnimalView viewPrefab;

        private IPool<Animal> _pool;
        private int _catCount;

        void Start()
        {
            _pool = new SimplePool<Animal>();

            _pool.Initialize(viewPrefab, 2);

            _catCount = 0;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                AddCat();
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                RemoveCat();
            }
            else if (Input.GetKeyDown(KeyCode.C))
            {
                _pool.Clean();
            }
        }

        private void AddCat()
        {
            string id = "Cat" + _catCount.ToString("00");
            Debug.Log("Adding " + id);

            var animal = new Animal();
            animal.Id = id;

            _catCount++;

            var view = _pool.Provide(animal);

            view.SceneObject.transform.position = new Vector3(1f * _catCount, 0f, 0.1f * _catCount);
        }

        private void RemoveCat()
        {
            _catCount--;

            string id = "Cat" + _catCount.ToString("00");

            if (_pool.Free(id))
            {
                Debug.Log("Removing " + id);
            }
            else
            {
                Debug.LogError("Failed to remove " + id);
            }
        }
    }
}

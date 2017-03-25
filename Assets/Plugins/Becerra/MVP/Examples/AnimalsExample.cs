using UnityEngine;
using Becerra.MVP.Model;
using Becerra.MVP.Views;
using Becerra.MVP.Pools;

namespace Becerra.MVP.Examples
{
    public class AnimalsExample : MonoBehaviour
    {
        public AnimalView viewPrefab;

        void Start()
        {
            var pool = new SimplePool<Animal>();

            pool.Initialize(viewPrefab, 2);

            var animal = new Animal();

            animal.Id = "Cat";

            var view = pool.Provide(animal);
        }
    }
}

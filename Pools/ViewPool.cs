using Becerra.MVP.Model;
using Becerra.MVP.Views;
using UnityEngine;

namespace Becerra.MVP.Pools
{
    /// <summary>
    /// Specialliced pool for views.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TR"></typeparam>
    public class ViewPool<T, TR> : Pool<TR>, IViewPool<T, TR> where T : class, IModel where TR: View<T>
    {
        public IUpdatableView BaseViewPrefab { get; private set; }
        public View<T> ViewPrefab { get; private set; }
        
        public ViewPool(TR prefab, int initialCount, Transform container) 
            : base(prefab, initialCount, container)
        {
            BaseViewPrefab = prefab.GetComponent<IUpdatableView>();
            ViewPrefab = prefab.GetComponent<View<T>>();
        }

        /// <inheritdoc />
        public IUpdatableView Provide(IModel model)
        {
            return Provide(model as T);
        }

        /// <inheritdoc />
        public TR Provide(T model)
        {
            var obj = Provide();

            var view = obj.GetComponent<TR>();

            view.ViewDisposer = this;
            view.Refresh(model);
            
            return view;
        }

        /// <inheritdoc />
        public bool Dispose(IUpdatableView<T> view)
        {
            return base.Dispose(view as TR);
        }

        /// <inheritdoc />
        public bool Dispose(T model)
        {
            return Dispose(model.Id);
        }

        /// <inheritdoc />
        public bool Dispose(IUpdatableView view)
        {
            return Dispose(view as TR);
        }

        /// <inheritdoc />
        public bool Dispose(IModel model)
        {
            return Dispose(model as T);
        }

        /// <inheritdoc />
        public bool Dispose(string id)
        {
            var view = Find(id);

            if (view == null) return false;

            view.Clean();
            
            return base.Dispose(view as TR);
        }

        /// <inheritdoc />
        public IUpdatableView Find(string id)
        {
            foreach (var view in this)
            {
                if (view.Model.Id == id) return view;
            }

            return null;
        }

        /// <inheritdoc />
        public IUpdatableView Find(IModel model)
        {
            return Find(model.Id);
        }

        /// <inheritdoc />
        public TR Find(T model)
        {
            return Find(model.Id) as TR;
        }

        public override void Dispose()
        {
            foreach (var view in UsedElements)
            {
                view.Clean();
            }
            
            base.Dispose();
        }
    }
}
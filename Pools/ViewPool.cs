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
        public View<T> Provide(T model)
        {
            var obj = Provide();

            var view = obj.GetComponent<View<T>>();
            
            view.Refresh(model);
            
            return view;
        }

        /// <inheritdoc />
        public bool Free(IUpdatableView<T> view)
        {
            return base.Free(view as TR);
        }

        /// <inheritdoc />
        public bool Free(T model)
        {
            return Free(model.Id);
        }

        /// <inheritdoc />
        public bool Free(IUpdatableView view)
        {
            return Free(view as TR);
        }

        /// <inheritdoc />
        public bool Free(IModel model)
        {
            return Free(model as T);
        }

        /// <inheritdoc />
        public bool Free(string id)
        {
            var view = Find(id);

            if (view == null) return false;

            view.Clean();
            
            return base.Free(view as TR);
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
        public View<T> Find(T model)
        {
            return Find(model.Id) as View<T>;
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
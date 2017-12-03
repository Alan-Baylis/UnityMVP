using Becerra.MVP.Model;
using Becerra.MVP.Views;
using UnityEngine;

namespace Becerra.MVP.Pools
{
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

        public IUpdatableView Provide(IModel model)
        {
            return Provide(model as T);
        }

        public View<T> Provide(T model)
        {
            var obj = Provide();

            var view = obj.GetComponent<View<T>>();
            
            view.Refresh(model);
            
            return view;
        }

        public bool Free(IUpdatableView<T> view)
        {
            return base.Free(view as TR);
        }

        public bool Free(T model)
        {
            return Free(model.Id);
        }

        public bool Free(IUpdatableView view)
        {
            return Free(view as TR);
        }

        public bool Free(IModel model)
        {
            return Free(model as T);
        }

        public bool Free(string id)
        {
            var view = Find(id);

            if (view == null) return false;

            return base.Free(view as TR);
        }

        public IUpdatableView Find(string id)
        {
            foreach (var view in this)
            {
                if (view.Model.Id == id) return view;
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
    }
}
using System;
using UnityEngine;

namespace MVVMToolkit.DataBinding
{
    public class CollectionBinder : Binder, ICollectionBinder, IViewFactory
    {
        [SerializeField] protected GameObject prefab;
        [SerializeField] protected Transform container;

        public event Action OnItemsAdded;
        public event Action OnItemsRemoved;

        protected override IBinding CreateBinding()
        {
            return new CollectionBinding(Path, this, this);
        }

        #region IViewFactory implementation

        public virtual GameObject CreateView(GameObject viewPrefab = null, int index = -1)
        {
            // Create new view
            GameObject view = Instantiate(prefab, container);

            // Set hierarchy
            if (index != -1)
                view.transform.SetSiblingIndex(index);
            else
                view.transform.SetAsLastSibling();

            view.name = "[UI] " + prefab.name;

            // Active view if not
            if (!view.activeSelf)
                view.SetActive(true);

            return view;
        }

        public virtual void ReleaseView(GameObject view)
        {
            Destroy(view);
        }

        #endregion

        public override void InitValue(object value)
        {
            throw new System.NotImplementedException();
        }

        public override void UpdateValue(object value)
        {
            throw new System.NotImplementedException();
        }

        public void ItemsAdded()
        {
            OnItemsAdded?.Invoke();
        }

        public void ItemsRemoved()
        {
            OnItemsRemoved?.Invoke();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MVVMToolkit.DataBinding
{
    public struct CollectionViewRef
    {
        public IDataContext DataContext { get; set; }
        public GameObject View { get; set; }
    }

    public class CollectionBinding : IBinding, ICollectionBinding
    {
        protected bool awaitInit;
        protected IDataContext source;
        protected IBinder target;
        protected IViewFactory viewFactory;
        protected string path;
        protected string propertyName;
        protected Dictionary<object, CollectionViewRef> viewsDictionary;
        protected BindingMode mode;
        protected UpdateSourceTrigger trigger;
        protected List<Transform> sortedViews;

#if UNITY_EDITOR
        protected List<object> valuesHistory;
#endif

        public CollectionBinding(string path, IBinder target, IViewFactory viewFactory)
        {
            if (viewFactory == null)
            {
                Debug.LogError("Target is null.");
                return;
            }

            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("Path is empty.");
                return;
            }

            if (target == null)
                throw new ArgumentNullException(nameof(target));

            this.path = path;
            this.target = target;
            this.viewFactory = viewFactory;
            propertyName = BindingUtility.GetPropertyName(path);
            viewsDictionary = new Dictionary<object, CollectionViewRef>();
            sortedViews = new List<Transform>();

#if UNITY_EDITOR
            valuesHistory = new List<object>();
#endif
        }

        public Dictionary<object, CollectionViewRef> GetPrefabs()
        {
            return viewsDictionary;
        }

        #region Item methods

        private (Transform, object) AddItem(object item, int index = -1)
        {
            // Create view
            GameObject prefab = null;
            object prefabProperty = BindingUtility.GetPropertyValue(item, "Prefab", true);
            if (prefabProperty != null && prefabProperty is GameObject go)
                prefab = go;

            GameObject view = viewFactory.CreateView(prefab, index);

            // Set source
            IDataContext dataContext = view.GetComponent<IDataContext>();
            if (dataContext != null)
            {
                dataContext.Data = item;

                viewsDictionary.Add(item, new CollectionViewRef
                {
                    DataContext = dataContext,
                    View = view
                });

                /*if (prefabAddressNode != null)
                {
                    prefabAddressNode.PropertyChanged += (o, e) =>
                    {
                        if (view != null)
                        {
                            int index = view.transform.GetSiblingIndex();

                            // Force replace (whole item changed || prefab)
                            RemoveItem(item);
                            AddItem(item, index); // Preserve items index in hierarchy
                        }
                    };
                }*/
            }

#if UNITY_EDITOR
            valuesHistory.Add(item);
#endif
            return (view.transform, item);
        }

        private void RemoveItem(object item)
        {
            if (!viewsDictionary.TryGetValue(item, out CollectionViewRef value))
            {
                Debug.LogError($"Unknown item {item}");
                return;
            }

            // Release
            if (value.View != null)
            {
                sortedViews.Remove(value.View.transform);
                viewFactory.ReleaseView(value.View);
            }

            viewsDictionary.Remove(item);
        }

        private void AddItems(IEnumerable items, int startingIndex = -1)
        {
            try
            {
                List<(Transform, object)> results = new();

                foreach (object item in items)
                {
                    object reference = item; // Avoid passing iterator

                    results.Add(AddItem(reference));
                }

                if (results.Count != 0)
                {
                    if (startingIndex >= 0)
                        sortedViews.InsertRange(startingIndex, results.Select(x => x.Item1));
                    else
                        sortedViews.AddRange(results.Select(x => x.Item1));

                    // Debt: This was changing sibling index so items were not added as last!
                    // This class needs cleanup in general, this sorted views logic is outdated
                    /*for (int i = 0; i < sortedViews.Count; i++)
                        sortedViews[i].SetSiblingIndex(i);*/

                    Transform parent = sortedViews.FirstOrDefault()?.parent;
                    if (parent != null && parent is RectTransform rt)
                    {
                        LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
                    }
                }

                if (Target is ICollectionBinder collectionBinder)
                    collectionBinder.ItemsAdded();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                throw;
            }
        }

        private void RemoveItems(IEnumerable items)
        {
            foreach (object item in items)
                RemoveItem(item);

            if (Target is ICollectionBinder collectionBinder)
                collectionBinder.ItemsRemoved();
        }

        protected void ClearItems()
        {
            IEnumerable itemsToRemove = new List<object>(viewsDictionary.Keys);
            RemoveItems(itemsToRemove);

            viewsDictionary.Clear();
            sortedViews.Clear();
        }

        #endregion Item methods

        #region IBinding implementation

        public string Path => path;
        public string PropertyName => propertyName;
        public bool AsynchronousInit => awaitInit;
        public bool IsBound => source != null;
        public IDataContext Source => source;
        public IBinder Target => target;
        public BindingMode Mode => mode;
        public UpdateSourceTrigger Trigger => trigger;

#if UNITY_EDITOR
        public IList<object> ValuesHistory => valuesHistory.AsReadOnly();
#endif

        public void Bind(IDataContext source)
        {
            if (source == null)
                throw new ArgumentNullException("source is null");

            if (IsBound)
                Unbind();

            this.source = source;

            object collectionSource = BindingUtility.GetBindingObject(this.source.Data, path, Target);
            object collectionProperty = BindingUtility.GetPropertyValue(collectionSource, propertyName);

            if (collectionProperty != null)
            {
                if (collectionProperty is INotifyPropertyChanged notifyIface)
                    notifyIface.PropertyChanged += OnSourcePropertyChanged;

                if (collectionProperty is INotifyCollectionChanged notifyCollection)
                    notifyCollection.CollectionChanged += OnCollectionChanged;

                if (collectionProperty is IEnumerable itemsEnumerator)
                    AddItems(itemsEnumerator);
            }
            else
            {
                Debug.LogError("Unable to bind collection to path: " + path);
            }
        }

        public void Unbind()
        {
            if (!IsBound)
                return;

            object collectionSource = BindingUtility.GetBindingObject(source.Data, path, Target);
            object collectionProperty = BindingUtility.GetPropertyValue(collectionSource, propertyName);

            if (collectionProperty == null)
                return;

            if (collectionProperty is INotifyPropertyChanged notifyIface)
                notifyIface.PropertyChanged -= OnSourcePropertyChanged;

            if (collectionProperty is INotifyCollectionChanged notifyCollection)
                notifyCollection.CollectionChanged -= OnCollectionChanged;

            ClearItems();
            source = null;

#if UNITY_EDITOR
            valuesHistory.Clear();
#endif
        }

        public void UpdateProperty(object value)
        {
            throw new NotImplementedException();
        }

        #endregion

        protected void OnSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!IsBound)
                return;

            // In the future here we would handle change of collection itself
            // if someone changes reference -> new enumerator, clear and refresh items

            // Right now this gets invoked when Count property changes on collection, atm nothing should happen

            // Get collection object
            /*object collectionProperty = BindingUtility.GetBindingObject(this.source.Data, this.path);
            if (collectionProperty == null)
                return;

            // Note: This would clear items anytime Count property changes
            ClearItems();

            IEnumerable newItemsEnumerator = collectionProperty as IEnumerable;
            if (newItemsEnumerator != null)
                AddItems(newItemsEnumerator);*/
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AddItems(e.NewItems, e.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Move:
                    throw new NotImplementedException(); // TODO
                                                         //MoveItems(e.NewItems);
                                                         //break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveItems(e.OldItems);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    throw new NotImplementedException(); // TODO
                                                         //ReplaceItems(e.OldItems, e.NewItems);
                                                         //break;
                case NotifyCollectionChangedAction.Reset:
                    ClearItems();
                    break;
            }
        }
    }
}

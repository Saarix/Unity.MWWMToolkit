using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

namespace MVVMToolkit.DataBinding
{
    public class DynamicCollectionBinder : Binder, ICollectionBinder, IViewFactory, ILayoutGroup
    {
        [SerializeField] 
        protected RectTransform container;

        [Header("Scroll rect for cotainer")]
        [SerializeField]
        protected ScrollRect scrollRect;

        [SerializeField]
        protected bool autoScrollToBottom = false;

        private Coroutine autoScrollCoroutine;

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
            GameObject view = Instantiate(viewPrefab, container);

            // Set hierarchy
            if (index != -1)
                view.transform.SetSiblingIndex(index);
            else
                view.transform.SetAsLastSibling();

            view.name = "[UI] " + viewPrefab.name;

            // Active view if not
            if (!view.activeSelf)
                view.SetActive(true);

            if (autoScrollToBottom && scrollRect != null)
            {
                if (autoScrollCoroutine != null)
                {
                    StopCoroutine(autoScrollCoroutine);
                    autoScrollCoroutine = null;
                }

                autoScrollCoroutine = StartCoroutine(AutoScroll());
            }

            return view;
        }

        public void ReleaseView(GameObject view)
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

        private IEnumerator AutoScroll()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(container);

            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            scrollRect.verticalNormalizedPosition = 0;
        }

        public void SetLayoutHorizontal() { }

        public void SetLayoutVertical()
        {
            if (scrollRect != null)
                scrollRect.verticalNormalizedPosition = 0;
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

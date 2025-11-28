using MVVMToolkit.DataBinding;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace MVVMToolkit.UI
{
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollToBinder : Binder<ScrollRect>
    {
        [Header("Needs index of an item to be provided through binding.")]
        [SerializeField] private bool scrollToItem = false;
        [SerializeField] private bool scrollOnlyOnce = false;
        [SerializeField] private bool registerCollectionNotifier = true;

        private ICollectionBinder collectionBinder = null;
        private bool isInitialized = false;
        private int? itemIndex;
        private float? scrollPosition;
        private bool wasScrolled = false;

        #region Binder implementation

        protected override void Awake()
        {
            base.Awake();

            if (registerCollectionNotifier)
            {
                collectionBinder = Component.GetComponentInChildren<ICollectionBinder>();
                collectionBinder.OnItemsAdded += CollectionBinder_OnItemsAdded;
            }
        }

        protected override void OnDisable()
        {
            if (registerCollectionNotifier)
                collectionBinder.OnItemsAdded -= CollectionBinder_OnItemsAdded;

            base.OnDisable();
        }

        public override void InitValue(object value)
        {
            base.InitValue(value);
            UpdateValue(value);
        }

        public override void UpdateValue(object value)
        {
            base.UpdateValue(value);

            if (value != null)
            {
                if (scrollToItem)
                {
                    if (int.TryParse(value.ToString(), out int result))
                    {
                        itemIndex = result;
                        ScrollToPosition();
                    }
                    else
                    {
                        throw new FormatException($"[{GetType().Name}] value is in unsupported format! value: {value.GetType()} was not integer.");
                    }
                }
                else
                {
                    if (float.TryParse(value.ToString(), out float result))
                    {
                        scrollPosition = result;
                        ScrollToPosition();
                    }
                    else
                    {
                        throw new FormatException($"[{GetType().Name}] value is in unsupported format! value: {value.GetType()} unable to parse to float.");
                    }
                }   
            }
        }

        #endregion Binder implementation

        private void CollectionBinder_OnItemsAdded()
        {
            isInitialized = true;
            ScrollToPosition();
        }

        private void ScrollToPosition()
        {
            if (!scrollOnlyOnce || (scrollOnlyOnce && !wasScrolled))
            {
                if (registerCollectionNotifier && !isInitialized)
                    return;

                if (scrollToItem && itemIndex.HasValue)
                {
                    // Scroll down to the target element - target element will be first at the top if possible
                    RectTransform target = Component.content.GetChild(itemIndex.Value).GetComponent<RectTransform>();

                    Vector2 anchoredPosition = Component.content.anchoredPosition;
                    float scrollDelta = target.rect.height * itemIndex.Value;

                    anchoredPosition.y += scrollDelta;
                    Component.content.anchoredPosition = anchoredPosition;
                    wasScrolled = true;
                }
                else if (scrollPosition.HasValue)
                {
                    Vector2 anchoredPosition = Component.content.anchoredPosition;
                    anchoredPosition.y = scrollPosition.Value;

                    Component.content.anchoredPosition = anchoredPosition;
                    //Component.verticalNormalizedPosition = scrollPosition.Value; // this needs normalized position, might be useful in some cases
                    wasScrolled = true;
                }
            }
        }
    }
}

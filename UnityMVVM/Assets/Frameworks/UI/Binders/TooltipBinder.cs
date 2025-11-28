using MVVMToolkit.DataBinding;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVVMToolkit.UI
{
    public class TooltipBinder : Binder, IPointerDownHandler, IPointerClickHandler
    {
        #region Fields

        [SerializeField]
        protected Tooltip.RelativePosition position;

        [SerializeField]
        protected float xOffset;

        [SerializeField]
        protected float yOffset;

        [SerializeField]
        protected bool overridePosition;

        protected RectTransform rectTransform;
        protected bool isValueInitialized = false;

        private LocalizableString localizeString;

        #endregion

        #region Properties

        public RectTransform Rect => rectTransform ?? (rectTransform = GetComponent<RectTransform>());

        #endregion

        #region Binder implementation

        public override void InitValue(object value)
        {
            base.InitValue(value);
            UpdateValue(value);
        }

        public override void UpdateValue(object value)
        {
            base.UpdateValue(value);

            if (value == null)
                return;

            if (value is LocalizableString localizableString)
            {
                localizeString = localizableString;
                isValueInitialized = true;
            }
        }

        #endregion

        public void OnPointerClick(PointerEventData eventData)
        {
            ShowTooltip(eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            eventData.Use();
        }

        private void ShowTooltip(PointerEventData eventData)
        {
            if (!isValueInitialized)
                return;

            TooltipManager.Instance.ShowTooltip(localizeString, Rect, position, xOffset, yOffset, overridePosition);

            // Use up the event so it never bubbles up
            eventData.Use();
        }
    }
}

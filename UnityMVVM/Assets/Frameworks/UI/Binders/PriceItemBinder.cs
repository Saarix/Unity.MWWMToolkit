using MVVMToolkit.DataBinding;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MVVMToolkit.UI
{
    public class PriceItemBinder : MultiBinder, ILayoutGroup
    {
        [SerializeField] private float iconWidthAddition = 15f;
        [SerializeField] private Image icon;
        [SerializeField] private LayoutElement iconLayoutElement;
        [SerializeField] private TextMeshProUGUI labelRequiredValue;
        [SerializeField] private TextMeshProUGUI labelCurrentValue;
        [SerializeField] private TextMeshProUGUI labelSlash;

        private RectTransform rectTra;
        private Color originalColor;
        private PriceItem priceItem = null;

        protected override void Awake()
        {
            base.Awake();

            rectTra = GetComponent<RectTransform>();
            originalColor = labelRequiredValue.color;
        }

        public override void InitValue(string sourcePath, int sourceIndex, object value)
        {
            base.InitValue(sourcePath, sourceIndex, value);
            UpdateValue(sourcePath, sourceIndex, value);
        }

        public override void UpdateValue(string sourcePath, int sourceIndex, object value)
        {
            base.UpdateValue(sourcePath, sourceIndex, value);

            switch (sourcePath)
            {
                case ".":
                {
                    if (value != null && value is PriceItem priceItem)
                    {
                        this.priceItem = priceItem;
                        icon.sprite = priceItem.Icon;
                        labelCurrentValue.gameObject.SetActive(priceItem.ShowCurrentAmount);
                        labelSlash.gameObject.SetActive(priceItem.ShowCurrentAmount);

                        UpdateState(priceItem.HasEnough);
                    }
                    else
                    {
                        Debug.LogError($"[{GetType().Name}] Value provided was not in correct format, expected typ was 'Price'! Value={value}");
                    }
                }
                break;
                case "HasEnough":
                {
                    if (value != null && value is bool hasEnough)
                    {
                        UpdateState(hasEnough);
                    }
                    else
                    {
                        Debug.LogError($"[{GetType().Name}] Value provided was not in correct format, expected type was 'bool'! Value={value}");
                    }
                    break;
                }
            }
        }

        private void UpdateState(bool hasEnough)
        {
            if (priceItem == null)
                return;

            if (priceItem.ShowCurrentAmount)
            {
                if (hasEnough)
                    labelCurrentValue.color = originalColor;
                else
                    labelCurrentValue.color = priceItem.NotEnoughtColor;
            }
            else
            {
                if (hasEnough)
                    labelRequiredValue.color = originalColor;
                else
                    labelRequiredValue.color = priceItem.NotEnoughtColor;
            }
        }

        public void SetLayoutHorizontal()
        {

        }

        public void SetLayoutVertical()
        {
            iconLayoutElement.preferredWidth = rectTra.rect.height + iconWidthAddition;
        }
    }
}

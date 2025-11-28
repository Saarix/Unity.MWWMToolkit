using MVVMToolkit.DataBinding;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MVVMToolkit.UI
{
    public class ProductPriceBinder : MultiBinder
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI labelLocalizedText;
        [SerializeField] private TextMeshProUGUI labelHardCurrency;
        [SerializeField] private TextMeshProUGUI labelRequiredAmount;

        private Color originalColor;
        private ProductPrice price = null;

        protected override void Awake()
        {
            base.Awake();

            originalColor = labelRequiredAmount.color;
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
                    if (value != null && value is ProductPrice price)
                    {
                        this.price = price;

                        switch (price.State)
                        {
                            // Hard currency
                            case 0:
                            {
                                icon.gameObject.SetActive(false);
                                labelRequiredAmount.gameObject.SetActive(false);
                                labelLocalizedText.gameObject.SetActive(false);
                                labelHardCurrency.gameObject.SetActive(true);
                            }
                            break;
                            // Ad currency
                            case 1:
                            {
                                icon.gameObject.SetActive(true);
                                labelRequiredAmount.gameObject.SetActive(false);
                                labelLocalizedText.gameObject.SetActive(true);
                                labelHardCurrency.gameObject.SetActive(false);
                            }
                            break;
                            // Virtual currency
                            case 2:
                            {
                                icon.gameObject.SetActive(true);
                                labelRequiredAmount.gameObject.SetActive(true);
                                labelLocalizedText.gameObject.SetActive(false);
                                labelHardCurrency.gameObject.SetActive(false);

                                UpdateState(price.HasEnough);
                            }
                            break;
                        }
                    }
                    else
                    {
                        Debug.LogError($"[{GetType().Name}] Value provided was not in correct format, expected type was 'Price'! Value={value}");
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
            if (price == null)
                return;

            if (hasEnough)
                labelRequiredAmount.color = originalColor;
            else
                labelRequiredAmount.color = price.NotEnoughtColor;
        }
    }
}

using MVVMToolkit.DataBinding;
using System;
using TMPro;
using UnityEngine;

namespace MVVMToolkit.UI
{
    public class NotificationBadgeBinder : Binder<TextMeshProUGUI>
    {
        public new TextMeshProUGUI Component
        {
            get
            {
                if (component == null)
                {
                    component = GetComponentInChildren<TextMeshProUGUI>();
                }

                return component;
            }
        }

        [SerializeField]
        private GameObject container;

        [SerializeField]
        private bool showCount = true;

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
                if (int.TryParse(value.ToString(), out int count))
                {
                    Component.text = count.ToString();
                    Component.gameObject.SetActive(showCount);
                    container.SetActive(count > 0);
                }
                else
                {
                    throw new FormatException($"{GetType().Name} value is not in supported type! type: {value.GetType()}");
                }
            }
        }
    }
}

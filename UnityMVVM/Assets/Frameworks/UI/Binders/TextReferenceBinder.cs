using MVVMToolkit.DataBinding;
using Geewa.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

namespace MVVMToolkit.UI
{
    public class TextReferenceBinder : StringBinder
    {
        [SerializeField] private TextMeshProUGUI textReference;
        [SerializeField] private bool shouldLocalize = false;

        private LocalizeStringEvent localizeStringEvent;

        protected override void Awake()
        {
            base.Awake();

            if (textReference != null)
                localizeStringEvent = textReference.GetComponent<LocalizeStringEvent>();
        }

        protected override IBinding CreateBinding()
        {
            // TODO: This is old, was for CustomTables with localization
            // Subscribe to collection changes for token updates to work!
            return new Binding(Path, this, fallbackValue, Converters, stringFormat, true);
        }
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

            if (!shouldLocalize)
            {
                textReference.text = value.ToString().Replace("<br>", "\r\n");
                return;
            }

            if (localizeStringEvent == null)
            {
                Debug.LogError($"Trying to localize string, but localizeStringEvent is null. value={value}");
                return;
            }

            if (value is LocalizableString localizableString)
            {
                if (!string.IsNullOrEmpty(localizableString.Table))
                {
                    localizeStringEvent.Set(localizableString.Key, localizableString.Table, localizableString.Tokens);
                }
                else
                {
                    localizeStringEvent.Set(localizableString.Key, localizableString.Tokens);
                }
            }
            else
            {
                localizeStringEvent.SetEntry(value.ToString());
            }
        }
    }
}

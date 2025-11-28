using TMPro;
using UnityEngine;
using MVVMToolkit.DataBinding;

namespace MVVMToolkit.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextMultiBinder : MultiBinder
    {
        [SerializeField]
        private string stringFormat;

        private TextMeshProUGUI label;
        public TextMeshProUGUI Label => label ?? (label = GetComponent<TextMeshProUGUI>());

        public override void InitValue(string sourcePath, int sourceIndex, object value)
        {
            UpdateValue(sourcePath, sourceIndex, value);
        }

        public override void UpdateValue(string sourcePath, int sourceIndex, object value)
        {
            base.UpdateValue(sourcePath, sourceIndex, value);

            if (value != null)
            {
                if (!string.IsNullOrEmpty(stringFormat))
                    Label.text = string.Format(stringFormat, values).Replace("<br>", "\r\n");
            }
        }
    }
}

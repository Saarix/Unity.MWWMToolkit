using MVVMToolkit.DataBinding;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace MVVMToolkit.UI
{
    public enum InputValidationRule
    {
        None,
        AlphaNumericWithSpaces
    }

    [RequireComponent(typeof(TMP_InputField))]
    public class TextInputBinder: StringBinder<TMP_InputField>
    {
        [SerializeField]
        protected InputValidationRule rule = InputValidationRule.None;

        protected override void Init()
        {
            base.Init();

            Component.onValidateInput += OnValidateInput;
        }

        protected override void Dispose()
        {
            base.Dispose();

            Component.onValidateInput -= OnValidateInput;
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
                Component.text = value.ToString();
        }

        private char OnValidateInput(string text, int charIndex, char addedChar) 
        {
            char val = addedChar;

            if (rule == InputValidationRule.AlphaNumericWithSpaces)
            {
                if (!Regex.IsMatch(val.ToString(), @"^[a-zA-Z0-9\s]+$"))
                    val = '\0';
            }

            return val;
        }
    }
}

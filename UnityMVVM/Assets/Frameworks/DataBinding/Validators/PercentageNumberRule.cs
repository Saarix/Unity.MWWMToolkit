using System;
using System.Globalization;

namespace MVVMToolkit.DataBinding
{
    public class PercentageNumberRule : ValidationRule
    {
        public float Min { get; set; }
        public float Max { get; set; }

        public PercentageNumberRule() { }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            float percentage = 0;
            
            try
            {
                if (((string)value).Length > 0)
                    percentage = float.Parse((string)value);
            }
            catch (Exception e)
            {
                return new ValidationResult(false, $"Illegal characters or {e.Message}");
            }

            if (percentage < Min || percentage > Max)
                return new ValidationResult(false, $"Please enter a percentage number in the range: {Min}-{Max}.");

            return ValidationResult.ValidResult;
        }
    }
}

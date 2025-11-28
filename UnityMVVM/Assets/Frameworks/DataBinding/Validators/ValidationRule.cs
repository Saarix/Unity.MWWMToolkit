using System.Globalization;
using UnityEngine;

namespace MVVMToolkit.DataBinding
{
    public enum ValidationStep
    {
        ConvertedProposedValue = 1, // Runs the ValidationRule after the value is converted.
        RawProposedValue = 0, // Runs the ValidationRule before any conversion occurs.
        UpdatedValue = 2 // Runs the ValidationRule after the source is updated.
                         // This will probably not be used in out flow
    }

    public abstract class ValidationRule : MonoBehaviour
    {
        private ValidationStep validationStep;
        private bool validatesOnTargetUpdated;

        public ValidationStep ValidationStep
        {
            get { return this.validationStep; }
            set { this.validationStep = value; }
        }

        public bool ValidatesOnTargetUpdated
        {
            get { return this.validatesOnTargetUpdated; }
            set { this.validatesOnTargetUpdated = value; }
        }

        protected ValidationRule() : this(ValidationStep.RawProposedValue, false) { }

        protected ValidationRule(ValidationStep validationStep, bool validatesOnTargetUpdated)
        {
            this.validationStep = validationStep;
            this.validatesOnTargetUpdated = validatesOnTargetUpdated;
        }

        public abstract ValidationResult Validate(object value, CultureInfo cultureInfo);
    }
}

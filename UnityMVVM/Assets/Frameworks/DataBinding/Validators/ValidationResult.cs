using UnityEngine;

namespace MVVMToolkit.DataBinding
{
    public class ValidationResult : MonoBehaviour
    {
        private bool isValid;
        private object errorContent;

        private static readonly ValidationResult validResult = new(true, null);

        public bool IsValid => this.isValid;
        public object ErrorContent => this.errorContent;
        public static ValidationResult ValidResult => validResult;

        public ValidationResult(bool isValid, object errorContent)
        {
            this.isValid = isValid;
            this.errorContent = errorContent;
        }

        public static bool operator ==(ValidationResult left, ValidationResult right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ValidationResult left, ValidationResult right)
        {
            return !Equals(left, right);
        }

        public override bool Equals(object obj)
        {
            // A cheaper alternative to Object.ReferenceEquals() is used here for better perf 
            if (obj == (object)this)
            {
                return true;
            }
            else
            {
                ValidationResult vr = obj as ValidationResult;
                if (vr != null)
                {
                    return (IsValid == vr.IsValid) && (ErrorContent == vr.ErrorContent);
                }
            }

            return false;
        }

        public override int GetHashCode()
        {
            return IsValid.GetHashCode() ^ (ErrorContent ?? int.MinValue).GetHashCode();
        }
    }
}

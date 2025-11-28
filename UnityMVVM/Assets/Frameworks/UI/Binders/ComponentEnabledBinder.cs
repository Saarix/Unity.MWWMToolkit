using MVVMToolkit.DataBinding;
using UnityEngine;

namespace MVVMToolkit.UI
{
    public class ComponentEnabledBinder : Binder
    {
        [SerializeField] private bool negateBool;
        [SerializeField] private Behaviour[] components;

        #region Binder implementation

        public override void InitValue(object value)
        {
            base.InitValue(value);
            UpdateValue(value);
        }

        public override void UpdateValue(object value)
        {
            base.UpdateValue(value);

            if (value is bool state)
            {
                foreach (Behaviour item in components)
                    item.enabled = negateBool ? !state : state;
            }
            else
            {
                Debug.LogError($"[{GetType().Name}] Given value is not a bool. Path: {Path}. gameObject: {gameObject.name}, value is {value}");
            }
        }

        #endregion
    }
}

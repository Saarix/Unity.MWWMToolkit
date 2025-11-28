using MVVMToolkit.DataBinding;
using UnityEngine;

namespace MVVMToolkit.UI
{
    [RequireComponent(typeof(Target))]
    public class TargetBinder : Binder<Target>
    {
        #region Binder implementation

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

            if (value is string targetKey)
            {
                Component.TargetKey = targetKey;
            }
            else
            {
                Debug.LogError($"[{GetType().Name}] Trying to use different type than Sprite. value is {value}, gameObject: {gameObject.name}");
            }
        }

        #endregion Binder implementation
    }
}

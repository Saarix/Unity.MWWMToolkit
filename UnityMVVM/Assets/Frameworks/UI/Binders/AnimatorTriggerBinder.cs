using UnityEngine;
using MVVMToolkit.DataBinding;

namespace MVVMToolkit.UI
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorTriggerBinder : Binder
    {
        /*
         * TODO:
         * Later on re-work to be a multi binder and support
         * all setable params of animator
         */

        private Animator animator;
        public Animator Animator => animator ?? (animator = GetComponent<Animator>());

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
                if (bool.TryParse(value.ToString(), out bool result))
                {
                    if (result)
                        Animator.SetTrigger(propertyName);
                }
                else
                {
                    Debug.LogError($"[{GetType().Name}] value is in unsupported format! Path: {Path}, value: {value.GetType()}");
                }
            }
        }
    }
}

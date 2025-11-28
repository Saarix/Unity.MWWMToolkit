using JoshH.UI;
using MVVMToolkit.DataBinding;
using UnityEngine;

namespace MVVMToolkit.UI
{
    [RequireComponent(typeof(UIGradient))]
    public class GradientBinder : Binder<UIGradient>
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

            if (value is Gradient gradient)
            {
                switch (Component.GradientType)
                {
                    case UIGradient.UIGradientType.Linear:
                    {
                        if (gradient.colorKeys.Length >= 2)
                        {
                            Component.LinearColor1 = gradient.colorKeys[0].color;
                            Component.LinearColor2 = gradient.colorKeys[1].color;
                        }
                    }

                    break;
                    case UIGradient.UIGradientType.ComplexLinear:
                    {
                        Component.LinearGradient = gradient;
                    }

                    break;
                }
            }
            else
            {
                Debug.LogError($"[{GetType().Name}] Trying to use different type than Gradient. value is {value}, gameObject: {gameObject.name}");
            }
        }

        #endregion Binder implementation
    }
}

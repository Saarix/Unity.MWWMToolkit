using System;
using MVVMToolkit.DataBinding;
using UnityEngine;

namespace MVVMToolkit.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasGroupBinder : Binder<CanvasGroup>
    {
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
                try
                {
                    if (float.TryParse(value.ToString(), out float val))
                    {
                        Component.alpha = val;
                    }
                    else
                    {
                        Debug.LogError($"CanvasGroupBinder - invalid value type: '{value?.GetType()}' ({value})!");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[{GetType().Name}] Failed - gameObject: {gameObject.name}, exception: {ex}");
                }
            }
        }
    }
}

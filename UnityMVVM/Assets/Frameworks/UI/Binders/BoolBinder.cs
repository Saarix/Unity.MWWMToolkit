using System;
using MVVMToolkit.DataBinding;
using UnityEngine;

namespace MVVMToolkit.UI
{
    public class BoolBinder : Binder
    {
        [Tooltip("Bool will be applied in positive way to Acvite property of each GO.")]
        [SerializeField]
        private GameObject[] positiveList;

        [Tooltip("Bool will be applied in negated way to Acvite property of each GO.")]
        [SerializeField]
        private GameObject[] negatedList;

        protected override bool StartSetFallbackValue()
        {
            if (!base.StartSetFallbackValue())
                return false;

            fallbackValue = false.ToString();
            return true;
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
            {
                try
                {
                    if (bool.TryParse(value.ToString(), out bool val))
                    {
                        foreach (var item in positiveList)
                            item.SetActive(val);

                        foreach (var item in negatedList)
                            item.SetActive(!val);
                    }
                    else
                    {
                        Debug.LogError($"BoolBinder - invalid value type: '{value?.GetType()}' ({value})!");
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

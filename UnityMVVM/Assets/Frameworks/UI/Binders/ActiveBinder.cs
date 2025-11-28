using System;
using UnityEngine;
using MVVMToolkit.DataBinding;

namespace MVVMToolkit.UI
{
    public class ActiveBinder : Binder
    {
        [SerializeField]
        private GameObject[] states;

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
                if (states == null || states.Length == 0)
                    return;

                if (value is int || value is long)
                {
                    int index = Convert.ToInt32(value);

                    for (int i = 0; i < states.Length; i++) // TOH: States could be duplicit
                        states[i].SetActive(false);

                    if (index < states.Length)
                        states[index].SetActive(true);
                }
                else
                {
                    Debug.LogError($"ActiveBinder - invalid value type: '{value?.GetType()}' ({value})!");
                }
            }
        }
    }
}

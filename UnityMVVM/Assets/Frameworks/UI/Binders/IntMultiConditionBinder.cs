using System.Linq;
using MVVMToolkit.DataBinding;
using UnityEngine;

namespace MVVMToolkit.UI
{
    public class IntMultiConditionBinder : Binder
    {
        [SerializeField]
        private int[] conditionValues;

        [Tooltip("Condition will be applied in positive way to Active property of each GO.")]
        [SerializeField]
        private GameObject[] positiveList;

        [Tooltip("Condition will be applied in negated way to Active property of each GO.")]
        [SerializeField]
        private GameObject[] negatedList;

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
                if (int.TryParse(value.ToString(), out int localValue))
                {
                    bool result = conditionValues != null && conditionValues.Contains(localValue);

                    foreach (var item in positiveList)
                        item.SetActive(result);

                    foreach (var item in negatedList)
                        item.SetActive(!result);
                }
                else
                {
                    Debug.LogError($"IntMultiConditionBinder - invalid value type: {value?.GetType()}, value: {value ?? "null"}! For path: {Path}, object: {gameObject.name}");
                }
            }
        }
    }
}

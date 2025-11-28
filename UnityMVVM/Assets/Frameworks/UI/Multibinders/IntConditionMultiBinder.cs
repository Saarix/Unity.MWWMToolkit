using UnityEngine;
using MVVMToolkit.DataBinding;

namespace MVVMToolkit.UI
{
    public class IntConditionMultiBinder : MultiBinder
    {
        [SerializeField]
        private IntConditionBinder.IntConditionType conditionType;

        [Tooltip("Condition will be applied in positive way to Acvite property of each GO.")]
        [SerializeField]
        private GameObject[] positiveList;

        [Tooltip("Condition will be applied in negated way to Acvite property of each GO.")]
        [SerializeField]
        private GameObject[] negatedList;

        private int? leftNumber, rightNumber;

        public override void InitValue(string sourcePath, int sourceIndex, object value)
        {
            base.InitValue(sourcePath, sourceIndex, value);
            UpdateValue(sourcePath, sourceIndex, value);
        }

        public override void UpdateValue(string sourcePath, int sourceIndex, object value)
        {
            base.UpdateValue(sourcePath, sourceIndex, value);

            if (value != null)
            {
                if (int.TryParse(value.ToString(), out int localValue))
                {
                    switch (sourceIndex)
                    {
                        case 0:
                            leftNumber = localValue;
                            break;
                        case 1:
                            rightNumber = localValue;
                            break;
                    }

                    if (leftNumber.HasValue && rightNumber.HasValue)
                    {
                        bool result = conditionType switch
                        {
                            IntConditionBinder.IntConditionType.Equals => leftNumber == rightNumber,
                            IntConditionBinder.IntConditionType.Greater => leftNumber > rightNumber,
                            IntConditionBinder.IntConditionType.Less => leftNumber < rightNumber,
                            IntConditionBinder.IntConditionType.GreaterOrEqual => leftNumber >= rightNumber,
                            IntConditionBinder.IntConditionType.LessOrEqual => leftNumber <= rightNumber,
                            _ => false
                        };

                        foreach (var item in positiveList)
                            item.SetActive(result);

                        foreach (var item in negatedList)
                            item.SetActive(!result);
                    }
                }
                else
                {
                    Debug.LogError($"IntConditionBinder - invalid value type: {value?.GetType()}, value: {value ?? "null"}! For path: {sourcePath}, object: {gameObject.name}");
                }
            }
        }
    }
}

using MVVMToolkit.DataBinding;
using UnityEngine;

namespace MVVMToolkit.UI
{
    public class IntConditionBinder : Binder
    {
        #region Nested stuff

        public enum IntConditionType
        {
            Equals,
            Greater,
            Less,
            GreaterOrEqual,
            LessOrEqual,
            NotEquals
        }

        #endregion

        [SerializeField]
        private int conditionValue;

        [SerializeField]
        private IntConditionType conditionType;

        [Tooltip("Condition will be applied in positive way to Acvite property of each GO.")]
        [SerializeField]
        private GameObject[] positiveList;

        [Tooltip("Condition will be applied in negated way to Acvite property of each GO.")]
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
                    bool result = conditionType switch
                    {
                        IntConditionType.Equals => conditionValue == localValue,
                        IntConditionType.Greater => conditionValue > localValue,
                        IntConditionType.Less => conditionValue < localValue,
                        IntConditionType.GreaterOrEqual => conditionValue >= localValue,
                        IntConditionType.LessOrEqual => conditionValue <= localValue,
                        IntConditionType.NotEquals => conditionValue != localValue,
                        _ => false
                    };

                    foreach (var item in positiveList)
                        item.SetActive(result);

                    foreach (var item in negatedList)
                        item.SetActive(!result);
                }
                else
                {
                    Debug.LogError($"IntConditionBinder - invalid value type: {value?.GetType()}, value: {value ?? "null"}! For path: {Path}, object: {gameObject.name}");
                }
            }
        }
    }
}

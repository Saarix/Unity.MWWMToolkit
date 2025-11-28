using System;
using System.Collections;
using TMPro;
using UnityEngine;
using MVVMToolkit.DataBinding;

namespace MVVMToolkit.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    [RequireComponent(typeof(AnimatedNumber))]
    public class AnimatedNumberMultiBInder : MultiBinder
    {
        [SerializeField]
        private bool useDelay = false;

        [SerializeField]
        private float delayTime = 0f;

        private AnimatedNumber animator;
        private TextMeshProUGUI label;

        public TextMeshProUGUI Label => label ?? (label = GetComponent<TextMeshProUGUI>());
        public AnimatedNumber Animator => animator ?? (animator = GetComponent<AnimatedNumber>());

        private float? oldValue, targetValue;
        private Coroutine delayCoroutine;

        public override void InitValue(string sourcePath, int sourceIndex, object value)
        {
            base.InitValue(sourcePath, sourceIndex, value);

            if (value != null)
                Animator.Init(ConvertValue(value));
        }

        public override void UpdateValue(string sourcePath, int sourceIndex, object value)
        {
            base.UpdateValue(sourcePath, sourceIndex, value);

            switch (sourcePath)
            {
                case "oldValue":
                    oldValue = ConvertValue(value);
                    break;
                case "targetValue":
                    targetValue = ConvertValue(value);
                    break;
            }

            if (oldValue.HasValue && targetValue.HasValue)
            {
                Animator.Init(oldValue.Value);

                if (useDelay)
                {
                    if (delayCoroutine != null)
                    {
                        StopCoroutine(delayCoroutine);
                        delayCoroutine = null;
                    }

                    delayCoroutine = StartCoroutine(DelayTransition(targetValue.Value));
                }
                else
                    Animator.SetNumber(targetValue.Value);
            }
        }

        private float ConvertValue(object value)
        {
            switch (value)
            {
                case int _:
                case long _:
                case float _:
                case double _:
                {
                    if (float.TryParse(value.ToString(), out float result))
                        return result;
                    else
                        throw new InvalidCastException($"[{GetType().Name}] Unsuccessful value cast, value type: {value.GetType()}");
                }
                default:
                    throw new NotImplementedException($"[{GetType().Name}] This value type is not supported: {value.GetType()}");
            }
        }

        private IEnumerator DelayTransition(float targetValue)
        {
            yield return new WaitForSeconds(delayTime);

            Animator.SetNumber(targetValue);
        }
    }
}

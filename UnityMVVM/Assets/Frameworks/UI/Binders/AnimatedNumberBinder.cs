using System;
using MVVMToolkit.DataBinding;
using TMPro;
using UnityEngine;

namespace MVVMToolkit.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    [RequireComponent(typeof(AnimatedNumber))]
    public class AnimatedNumberBinder : Binder
    {
        [SerializeField] private bool firstSetInstant = false;

        private AnimatedNumber animator;
        private TextMeshProUGUI label;

        public TextMeshProUGUI Label => label ?? (label = GetComponent<TextMeshProUGUI>());
        public AnimatedNumber Animator => animator ?? (animator = GetComponent<AnimatedNumber>());

        private bool wasSetOnce = false;
        private bool wasInitialized = false;

        public override void InitValue(object value)
        {
            base.InitValue(value);

            if (value != null)
            {
                wasInitialized = true;
                Animator.Init(ConvertValue(value));
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            wasInitialized = false;
            wasSetOnce = false;
        }

        public override void UpdateValue(object value)
        {
            base.UpdateValue(value);

            if (value != null)
            {
                if (firstSetInstant && wasInitialized && !wasSetOnce)
                {
                    wasSetOnce = true;
                    Animator.Init(ConvertValue(value));
                }
                else
                {
                    Animator.SetNumber(ConvertValue(value));
                }
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
    }
}

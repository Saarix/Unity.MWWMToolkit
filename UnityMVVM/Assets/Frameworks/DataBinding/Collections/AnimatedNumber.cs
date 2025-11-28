using System;
using TMPro;
using UnityEngine;

namespace MVVMToolkit.DataBinding
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class AnimatedNumber : MonoBehaviour
    {
        [Tooltip("Duration of the animation in seconds.")]
        [SerializeField]
        private float duration = 1f;

        [Tooltip("Converter to format the number.")]
        [SerializeField]
        private ValueConverter converter;

        private TextMeshProUGUI label;
        private float value;
        private float oldValue;
        private float targetValue;
        private bool timerIsRunning = false;
        private float timeRemaining;

        public TextMeshProUGUI Label => label ?? (label = GetComponent<TextMeshProUGUI>());
        public event Action OnAnimationFinished;

        private void Update()
        {
            if (timerIsRunning)
            {
                if (timeRemaining > 0)
                {
                    timeRemaining -= Time.deltaTime;
                    float t = (duration - timeRemaining) / duration;
                    UpdateValue((int)Mathf.SmoothStep(oldValue, targetValue, t));
                }
                else
                {
                    UpdateValue((int)targetValue);
                    timeRemaining = 0;
                    timerIsRunning = false;
                    OnAnimationFinished?.Invoke();
                }
            }
        }

        private void UpdateValue(int value)
        {
            this.value = value;
            if (converter != null)
                Label.text = converter.Convert(value, typeof(string), null, null).ToString();
            else
                Label.text = value.ToString();
        }

        public void Init(float value)
        {
            timerIsRunning = false;
            targetValue = value;
            oldValue = value;

            UpdateValue((int)value);
        }

        public void SetNumber(float value, float? duration = null)
        {
            oldValue = this.value;

            if (oldValue == value)
                return;

            targetValue = value;
            timeRemaining = duration.HasValue ? duration.Value : this.duration;
            timerIsRunning = true;
        }
    }
}
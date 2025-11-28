using MVVMToolkit.DataBinding;
using UnityEngine;
using UnityEngine.UI;

namespace MVVMToolkit.UI
{
    [RequireComponent(typeof(Slider))]
    public class SliderInputBinder : Binder<Slider>
    {
        [SerializeField]
        private float step = 1f;

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

            float val = Component.value;

            if (value is int num)
                val = num;
            else if (value is float fnum)
                val = fnum;

            Component.value = Step(val, step);
        }

        static float Step(float value, float step)
        {
            float absValue = Mathf.Abs(value);
            step = Mathf.Abs(step);

            float low = absValue - (absValue % step);
            float high = low + step;

            float result = absValue - low < high - absValue ? low : high;

            return result * Mathf.Sign(value);
        }
    }
}

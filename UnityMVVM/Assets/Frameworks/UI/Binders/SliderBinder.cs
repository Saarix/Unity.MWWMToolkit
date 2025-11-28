using MVVMToolkit.DataBinding;
using UnityEngine;
using UnityEngine.UI;

namespace MVVMToolkit.UI
{
    [RequireComponent(typeof(Slider))]
    public class SliderBinder : Binder<Slider>
    {
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

            if (value is float fnum)
                Component.value = fnum;
        }
    }
}

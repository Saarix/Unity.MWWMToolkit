using MVVMToolkit.DataBinding;
using UnityEngine;
using UnityEngine.UI;

namespace MVVMToolkit.UI
{
    [RequireComponent(typeof(Toggle))]
    public class ToggleBinder : Binder<Toggle>
    {
        [SerializeField] private AudioClip clickSfx;
        [SerializeField] private CanvasGroup onSection;
        [SerializeField] private CanvasGroup offSection;

        #region Binder implementation

        protected override void Init()
        {
            base.Init();

            Component.onValueChanged.AddListener(UpdateToggle);
        }

        protected override void Dispose()
        {
            Component.onValueChanged.RemoveListener(UpdateToggle);

            base.Dispose();
        }

        public override void InitValue(object value)
        {
            base.InitValue(value);
            UpdateValue(value);
        }

        public override void UpdateValue(object value)
        {
            base.UpdateValue(value);

            if (value is bool valBool)
                UpdateToggle(valBool);
            else
                Debug.LogError($"[{GetType().Name}] Given value is not a bool. gameObject: {gameObject.name}, value is {value}");
        }

        #endregion

        private void UpdateToggle(bool value)
        {
            Component.isOn = value;
            onSection.gameObject.SetActive(value);
            offSection.gameObject.SetActive(!value);
        }
    }
}

using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MVVMToolkit.DataBinding
{
    public interface IInputBinder
    {
        Action<object> OnUpdateValue { get; set; }
    }

    public abstract class Binder<T> : Binder, IDeselectHandler, IInputBinder, IPointerUpHandler
        where T : Component
    {
        #region Inspector fields

        [SerializeField]
        protected UpdateSourceTrigger trigger;

        #endregion Inspector fields

        protected T component;

        #region properties

        public UpdateSourceTrigger Trigger => this.trigger;

        public T Component
        {
            get
            {
                if (this.component == null)
                {
                    this.component = GetComponent<T>();
                }

                return this.component;
            }
        }

        public Action<object> OnUpdateValue { get; set; }

        #endregion

        protected override void Init()
        {
            if (Mode == BindingMode.TwoWay || Mode == BindingMode.OneWayToSource)
                RegisterComponentChangeCallbacks();

            base.Init();
        }

        protected override void Dispose()
        {
            if (Mode == BindingMode.TwoWay || Mode == BindingMode.OneWayToSource)
                UnregisterComponentChangeCallbacks();

            base.Dispose();
        }

        protected override IBinding CreateBinding()
        {
            return new Binding(Path, this, this.fallbackValue, Converters, mode: Mode, trigger: Trigger);
        }

        private void RegisterComponentChangeCallbacks()
        {
            switch (Component)
            {
                case TMP_InputField tmpInput:
                {
                    switch (Trigger)
                    {
                        case UpdateSourceTrigger.LostFocus:
                            tmpInput.onEndEdit.AddListener(ValueChanged);
                            break;
                        case UpdateSourceTrigger.PropertyChanged:
                            tmpInput.onValueChanged.AddListener(ValueChanged);
                            break;
                    }
                }

                break;
                case InputField field:
                {
                    switch (Trigger)
                    {
                        case UpdateSourceTrigger.LostFocus:
                            field.onEndEdit.AddListener(ValueChanged);
                            break;
                        case UpdateSourceTrigger.PropertyChanged:
                            field.onValueChanged.AddListener(ValueChanged);
                            break;
                    }
                }

                break;
                case Slider slider:
                {
                    switch (Trigger)
                    {
                        case UpdateSourceTrigger.LostFocus:
                            throw new NotImplementedException($"[{GetType().Name}] LostFocus trigger is not implemented for {Component.GetType().Name}");
                        case UpdateSourceTrigger.PropertyChanged:
                            slider.onValueChanged.AddListener((val) => ValueChanged(val));
                            break;
                    }
                }

                break;
                case Toggle toggle:
                {
                    switch (Trigger)
                    {
                        case UpdateSourceTrigger.LostFocus:
                            throw new NotImplementedException($"[{GetType().Name}] LostFocus trigger is not implemented for {Component.GetType().Name}");
                        case UpdateSourceTrigger.PropertyChanged:
                            toggle.onValueChanged.AddListener((val) => ValueChanged(val));
                            break;
                    }
                }

                break;
                case TMP_Dropdown tmpDropdown:
                {
                    switch (Trigger)
                    {
                        case UpdateSourceTrigger.LostFocus:
                            throw new NotImplementedException($"[{GetType().Name}] LostFocus trigger is not implemented for {Component.GetType().Name}");
                        case UpdateSourceTrigger.PropertyChanged:
                            tmpDropdown.onValueChanged.AddListener((val) => ValueChanged(val));
                            break;
                    }
                }

                break;
                case Dropdown dropdown:
                {
                    switch (Trigger)
                    {
                        case UpdateSourceTrigger.LostFocus:
                            throw new NotImplementedException($"[{GetType().Name}] LostFocus trigger is not implemented for {Component.GetType().Name}");
                        case UpdateSourceTrigger.PropertyChanged:
                            dropdown.onValueChanged.AddListener((val) => ValueChanged(val));
                            break;
                    }
                }

                break;
            }
        }

       
        private void UnregisterComponentChangeCallbacks()
        {
            switch (Component)
            {
                case TMP_InputField tmpInput:
                {
                    switch (Trigger)
                    {
                        case UpdateSourceTrigger.LostFocus:
                            tmpInput.onEndEdit.RemoveListener(ValueChanged);
                            break;
                        case UpdateSourceTrigger.PropertyChanged:
                            tmpInput.onValueChanged.RemoveListener(ValueChanged);
                            break;
                    }
                }

                break;
                case InputField field:
                {
                    switch (Trigger)
                    {
                        case UpdateSourceTrigger.LostFocus:
                            field.onEndEdit.RemoveListener(ValueChanged);
                            break;
                        case UpdateSourceTrigger.PropertyChanged:
                            field.onValueChanged.RemoveListener(ValueChanged);
                            break;
                    }
                }

                break;
                case Slider slider:
                {
                    switch (Trigger)
                    {
                        case UpdateSourceTrigger.LostFocus:
                            throw new NotImplementedException($"[{GetType().Name}] LostFocus trigger is not implemented for {Component.GetType().Name}");
                        case UpdateSourceTrigger.PropertyChanged:
                            slider.onValueChanged.RemoveListener((val) => ValueChanged(val));
                            break;
                    }
                }

                break;
                case Toggle toggle:
                {
                    switch (Trigger)
                    {
                        case UpdateSourceTrigger.LostFocus:
                            throw new NotImplementedException($"[{GetType().Name}] LostFocus trigger is not implemented for {Component.GetType().Name}");
                        case UpdateSourceTrigger.PropertyChanged:
                            toggle.onValueChanged.RemoveListener((val) => ValueChanged(val));
                            break;
                    }
                }

                break;
                case TMP_Dropdown tmpDropdown:
                {
                    switch (Trigger)
                    {
                        case UpdateSourceTrigger.LostFocus:
                            throw new NotImplementedException($"[{GetType().Name}] LostFocus trigger is not implemented for {Component.GetType().Name}");
                        case UpdateSourceTrigger.PropertyChanged:
                            tmpDropdown.onValueChanged.RemoveListener((val) => ValueChanged(val));
                            break;
                    }
                }

                break;
                case Dropdown dropdown:
                {
                    switch (Trigger)
                    {
                        case UpdateSourceTrigger.LostFocus:
                            throw new NotImplementedException($"[{GetType().Name}] LostFocus trigger is not implemented for {Component.GetType().Name}");
                        case UpdateSourceTrigger.PropertyChanged:
                            dropdown.onValueChanged.RemoveListener((val) => ValueChanged(val));
                            break;
                    }
                }

                break;
            }
        }

        private void ValueChanged(object value)
        {
            Binding.UpdateProperty(value);

            OnUpdateValue?.Invoke(value);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (eventData.selectedObject == this && Trigger == UpdateSourceTrigger.LostFocus)
            {
                switch (Component)
                {
                    case TMP_InputField tmpField:
                        ValueChanged(tmpField.text);
                        break;
                    case InputField field:
                        ValueChanged(field.text);
                        break;
                    case Slider slider:
                        ValueChanged(slider.value);
                        break;
                    case Toggle toggle:
                        ValueChanged(toggle.isOn);
                        break;
                    case TMP_Dropdown tmpDropdown:
                        ValueChanged(tmpDropdown.value);
                        break;
                    case Dropdown dropdown:
                        ValueChanged(dropdown.value);
                        break;
                }
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (Trigger == UpdateSourceTrigger.PointerUp)
            {
                switch (Component)
                {
                    case TMP_InputField tmpField:
                        ValueChanged(tmpField.text);
                        break;
                    case InputField field:
                        ValueChanged(field.text);
                        break;
                    case Slider slider:
                        ValueChanged(slider.value);
                        break;
                    case Toggle toggle:
                        ValueChanged(toggle.isOn);
                        break;
                    case TMP_Dropdown tmpDropdown:
                        ValueChanged(tmpDropdown.value);
                        break;
                    case Dropdown dropdown:
                        ValueChanged(dropdown.value);
                        break;
                }
            }
        }
    }
}

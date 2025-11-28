using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using UnityEngine;

namespace MVVMToolkit.DataBinding
{
    public enum BindingMode
    {
        OneWay,
        OneTime,
        TwoWay,
        OneWayToSource
    }

    public enum UpdateSourceTrigger
    {
        PropertyChanged,
        LostFocus,
        Explicit,
        PointerUp
    }

    /// <summary>
    /// If you set the Converter and StringFormat properties, the converter is applied to the data value first, and then the StringFormat is applied.
    /// </summary>
    public class Binding : IBinding
    {
        protected IDataContext source;
        protected IBinder target;
        protected string path;
        protected object fallbackValue;
        protected IList<ValueConverter> converters;
        protected ValidationRule validator;
        protected string stringFormat;
        protected bool subscribeCollectionChanged;
        protected BindingMode mode;
        protected UpdateSourceTrigger trigger;
        protected object bindingObject;
        protected string propertyName;
        protected PropertyInfo propertyInfo;

        protected Action<ValidationResult> OnValidate;

#if UNITY_EDITOR
        protected List<object> valuesHistory;
#endif

        #region Ctors

        public Binding(string path, IBinder target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            this.path = path;
            this.target = target;
            propertyName = BindingUtility.GetPropertyName(path);

#if UNITY_EDITOR
            valuesHistory = new List<object>();
#endif
        }

        /// <summary>
        /// Constructs a binding relationship between DataContext Path (Property) and Binder implementation
        /// </summary>
        /// <param name="path">Path that is bound to in DataContext</param>
        /// <param name="target">Target binder that will be used in the relationship</param>
        /// <param name="fallbackValue">Fallback value to be used if no value is provided. Binders can define only fallback value.</param>
        /// <param name="converters">Converters to be used to convert the value. Value is converted before sent to the target binder.</param>
        /// <param name="stringFormat">String formatting to be used on the value. Formatting is performed before sent to the target binder.</param>
        /// <param name="subscribeCollectionChanged">Notifies root about nested property changes</param>
        /// <param name="mode">Defines binding mode that will be used in the relationship. Mostly OneWay or TwoWay for user input.</param>
        /// <param name="trigger">Defines trigger that triggers the update of when to propagate value forward. Used for TwoWay banding only.</param>
        /// <param name="validator">Used to validate the value. It validates the value with Validator rules before any formatting/convert is performed and sent to the target binder.</param>
        public Binding(string path, IBinder target, object fallbackValue = null, IList<ValueConverter> converters = null,
            string stringFormat = null, bool subscribeCollectionChanged = false, BindingMode mode = BindingMode.OneWay,
            UpdateSourceTrigger trigger = UpdateSourceTrigger.LostFocus, ValidationRule validator = null) : this(path, target)
        {
            this.fallbackValue = fallbackValue;
            this.converters = converters;
            this.stringFormat = stringFormat;
            this.subscribeCollectionChanged = subscribeCollectionChanged;
            this.mode = mode;
            this.trigger = trigger;
            this.validator = validator;
        }

        #endregion Ctors

        #region IBinding implementation

        public string Path => path;
        public string PropertyName => propertyName;
        public bool IsBound => source != null;
        public IDataContext Source => source;
        public IBinder Target => target;
        public BindingMode Mode => mode;
        public UpdateSourceTrigger Trigger => trigger;

#if UNITY_EDITOR
        public IList<object> ValuesHistory => valuesHistory.AsReadOnly();
#endif

        public void Bind(IDataContext source)
        {
            // No path -> use fallback value directly
            if (string.IsNullOrEmpty(path))
            {
                InitValueAsync(fallbackValue);
            }
            else
            {
                // Try to get property from path
                if (source == null)
                    throw new ArgumentNullException("source is null");

                if (IsBound)
                    Unbind();

                this.source = source;

                bindingObject = BindingUtility.GetBindingObject(this.source.Data, path, Target);
                if (bindingObject != null)
                {
                    propertyInfo = bindingObject.GetType().GetProperty(propertyName);

                    if (bindingObject is INotifyPropertyChanged notifyIface)
                        notifyIface.PropertyChanged += OnPropertyChangedAsync;

                    // Support for notifying root about nested prop changes
                    if (subscribeCollectionChanged)
                    {
                        if (bindingObject is INotifyCollectionChanged notifyCollectionIface)
                            notifyCollectionIface.CollectionChanged += OnCollectionChangedAsync;
                    }

                    InitValueAsync(BindingUtility.GetPropertyValue(bindingObject, propertyName));
                }
                else
                {
                    Debug.LogError($"Unable to bind to path: '{path}', for source: '{this.source}', for target: '{target}'");

                    InitValueAsync(fallbackValue);
                }
            }
        }

        public void Unbind()
        {
            if (!IsBound)
                return;

            if (bindingObject != null)
            {
                if (bindingObject is INotifyPropertyChanged notifyIface)
                    notifyIface.PropertyChanged -= OnPropertyChangedAsync;

                if (subscribeCollectionChanged)
                {
                    if (bindingObject is INotifyCollectionChanged notifyCollectionIface)
                        notifyCollectionIface.CollectionChanged -= OnCollectionChangedAsync;
                }

                bindingObject = null;
            }

            source = null;

#if UNITY_EDITOR
            valuesHistory.Clear();
#endif
        }

        public void UpdateProperty(object value)
        {
            if (!IsBound)
                return;

            if (bindingObject == null)
                Debug.LogError($"Unable to update property since it is null. For path={Path} with provided value:{value}");

            Type valueType = value.GetType();
            if (valueType == propertyInfo.PropertyType)
                propertyInfo.SetValue(bindingObject, value);
            else
                propertyInfo.SetValue(bindingObject, Convert.ChangeType(value, propertyInfo.PropertyType));
        }

        #endregion IBinding implementation

        protected void OnPropertyChangedAsync(object sender, PropertyChangedEventArgs e)
        {
            if (!IsBound)
                return;

            if (e.PropertyName != null && propertyName != e.PropertyName)
            {
                // ignore invalid source path
                return;
            }

            object value = BindingUtility.GetPropertyValue(sender, e.PropertyName);

            UpdateValue(value);
        }

        protected void OnCollectionChangedAsync(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!IsBound)
                return;

            //TODO: check if here we are sending correct object

            UpdateValue(sender);
        }

        private void InitValueAsync(object value)
        {
            if (validator != null && validator.ValidationStep == ValidationStep.RawProposedValue)
                OnValidate?.Invoke(validator.Validate(value, CultureInfo.InvariantCulture));

            value = FormatValue(value);

            if (validator != null && validator.ValidationStep == ValidationStep.ConvertedProposedValue)
                OnValidate?.Invoke(validator.Validate(value, CultureInfo.InvariantCulture));

#if UNITY_EDITOR
            valuesHistory.Add(value);
#endif
            target.InitValue(value);
        }

        private void UpdateValue(object value)
        {
            if (validator != null && validator.ValidationStep == ValidationStep.RawProposedValue)
                OnValidate?.Invoke(validator.Validate(value, CultureInfo.InvariantCulture));

            value = FormatValue(value);

            if (validator != null && validator.ValidationStep == ValidationStep.ConvertedProposedValue)
                OnValidate?.Invoke(validator.Validate(value, CultureInfo.InvariantCulture));

#if UNITY_EDITOR
            valuesHistory.Add(value);
#endif
            target.UpdateValue(value);
        }

        private object FormatValue(object value)
        {
            // Apply converters
            if (converters != null)
            {
                foreach (ValueConverter converter in converters)
                    value = converter.Convert(value, target.GetType(), null, CultureInfo.CurrentCulture);
            }

            // Apply string format
            if (!string.IsNullOrEmpty(stringFormat))
                value = string.Format(stringFormat, value).Replace("<br>", "\r\n");

            return value;
        }
    }
}
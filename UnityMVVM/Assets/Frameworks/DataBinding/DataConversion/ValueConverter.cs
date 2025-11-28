using System;
using System.Globalization;
using MVVMToolkit.DataBinding.Responses;
using UnityEngine;

namespace MVVMToolkit.DataBinding
{
    public abstract class ValueConverter:ScriptableObject
    {
        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);
        public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
    }

    public abstract class ValueConverter<T1,T2>:ValueConverter
    {
        [Serializable]
        private struct ConverterPair
        {
            [SerializeField] private T1 key;
            [SerializeField] private T2 value;

            public T1 Key => this.key;
            public T2 Value => this.value;
        }

        [SerializeField]private ConverterPair[] config;

        protected IDataConversionResponse GetValue(T1 key)
        {
            if (this.config == null)
                return new DataConversionError("No config loaded");

            foreach (ConverterPair pair in this.config)
            {
                if (pair.Key.Equals(key))
                    return new DataConversionOK(pair.Value);
            }

            return new DataConversionMissing(key);
        }

        protected IDataConversionResponse GetKey(T2 value)
        {
            if (this.config == null)
                return new DataConversionError("No config loaded");

            foreach (ConverterPair pair in this.config)
            {
                if (pair.Value.Equals(value))
                    return new DataConversionOK(pair.Key);
            }

            return new DataConversionMissing(value);
        }
    }
}

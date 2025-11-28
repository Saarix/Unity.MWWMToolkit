using System;
using System.Globalization;
using UnityEngine;

namespace MVVMToolkit.DataBinding
{
    [CreateAssetMenu(fileName = "Number To Char Converter", menuName = "Geewa/Converters/Number To Char Converter")]
    public class NumberToCharConverter : ValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringVal)
            {
                if (int.TryParse(stringVal, out int intVal))
                    return GetNumber(intVal);
                else if (float.TryParse(stringVal, out float floatVal))
                    return GetNumber(floatVal);
                else
                    return value;
            }
            else if (value is int numberInt)
            {
                return GetNumber(numberInt);
            }
            else if (value is long numberLong)
            {
                return GetNumber(numberLong);
            }
            else if (value is float numberFloat)
            {
                return GetNumber(numberFloat);
            }
            else if (value is double numberDouble)
            {
                return GetNumber(numberDouble);
            }
            else
            {
                return value;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }

        private string GetNumber(double value)
        {
            if (value >= 10000 && value < 1000000)
                return $"{value / 1000:#.##}K";
            else if (value >= 1000000 && value < 1000000000)
                return $"{value / 1000000:#.##}M";
            else if (value >= 1000000000)
                return $"{value / 1000000000:#.##}B";
            else
                return value.ToString();
        }
    }
}

using System;
using System.Globalization;
using UnityEngine;

namespace MVVMToolkit.DataBinding
{
    [CreateAssetMenu(fileName = "Upper Case Converter", menuName = "Geewa/Converters/Upper Case Converter")]
    public class UpperCaseConverter : ValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString().ToUpperInvariant();
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }
    }
}

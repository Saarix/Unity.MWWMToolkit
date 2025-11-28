using System;
using System.Globalization;
using UnityEngine;

namespace MVVMToolkit.DataBinding
{
    [CreateAssetMenu(fileName = "Lower Case Converter", menuName = "Geewa/Converters/Lower Case Converter")]
    public class LowerCaseConverter : ValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString().ToLowerInvariant();
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }
    }
}

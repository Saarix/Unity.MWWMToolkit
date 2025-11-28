using System;
using System.Globalization;
using UnityEngine;

namespace MVVMToolkit.DataBinding
{
    [CreateAssetMenu(fileName = "Negative Sign Converter", menuName = "Geewa/Converters/Negative Sign Converter")]
    public class NegativeSignConverter : ValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return $"-{value}";
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string chain = value.ToString();
            return chain[0] == '-' ? chain.Substring(1) : chain;
        }
    }
}

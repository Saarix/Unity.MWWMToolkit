using System;
using System.Globalization;
using MVVMToolkit.DataBinding.Responses;
using UnityEngine;

namespace MVVMToolkit.DataBinding
{
    [CreateAssetMenu(fileName = "Rarity To String Converter", menuName = "Geewa/Converters/Rarity To String Converter")]
    public class RarityToStringConverter : ValueConverter<int,string>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (int.TryParse(value.ToString(), out int result))
            {
                switch (GetValue(result))
                {
                    case DataConversionOK response:
                        return response.Data;
                    case IDataConversionError response:
                        throw new InvalidOperationException(response.Message);
                    default:
                        throw new NotImplementedException("Unknown response type!");
                }
            }
            else
            {
                throw new ArgumentException($"[{GetType().Name}] Value was not in correct format. Only 'int' format is supported! {value.GetType()} was provided.");
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            switch (GetKey(value.ToString()))
            {
                case DataConversionOK response:
                    return response.Data;
                case IDataConversionError response:
                    throw new InvalidOperationException(response.Message);
                default:
                    throw new NotImplementedException("Unknown response type!");
            }
        }
    }
}

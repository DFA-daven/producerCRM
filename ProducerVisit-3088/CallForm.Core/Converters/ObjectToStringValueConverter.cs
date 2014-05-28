using System;
using System.Globalization;
using Cirrious.CrossCore.Converters;

namespace CallForm.Core.Converters
{
    public class ObjectToStringValueConverter : MvxValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }
    }
}

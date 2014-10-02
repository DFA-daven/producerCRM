using System;
using System.Globalization;
using Cirrious.CrossCore.Converters;

namespace CallForm.Core.Converters
{
    #region overrides
    #pragma warning disable 1591
    public class ObjectToStringValueConverter : MvxValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }
    }
    #pragma warning restore 1591
    #endregion overrides
}

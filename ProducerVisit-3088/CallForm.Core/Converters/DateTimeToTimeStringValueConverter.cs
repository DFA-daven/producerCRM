using System;
using System.Globalization;
using Cirrious.CrossCore.Converters;

namespace CallForm.Core.Converters
{
    public class DateTimeToTimeStringValueConverter : MvxValueConverter<DateTime, string>
    {
        protected override string Convert(DateTime value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString("t");
        }
    }
}

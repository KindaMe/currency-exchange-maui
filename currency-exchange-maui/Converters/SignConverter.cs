using System.Globalization;

namespace currency_exchange_maui.Converters;

public class SignConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double numericValue)
        {
            if (numericValue > 0)
                return "Positive";
            else if (numericValue < 0)
                return "Negative";
            else
                return "Zero";
        }

        return "Undefined";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
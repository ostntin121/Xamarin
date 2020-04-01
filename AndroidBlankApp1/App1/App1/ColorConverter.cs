using System;
using System.Globalization;
using Xamarin.Forms;

namespace App1
{
    public class ColorConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!bool.TryParse(value.ToString(), out var b))
                return Color.DarkGray;
                
            switch (b)
            {
                case true: return Color.MediumSeaGreen;
                case false: return Color.Firebrick;
                default: return Color.DarkGray;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class HighlightColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!bool.TryParse(value.ToString(), out var b))
                return Color.White;

            return b ? Color.LightBlue : Color.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace MediaControlsLibrary.Converters
{
    public class SearchBoxMessageConverter : IValueConverter
    {
        // Converter (Show / Hide Text Based on Boolean Value)
        // ==============================================================
        // ==============================================================
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) { return (bool)value ? Visibility.Visible : Visibility.Collapsed; }


        #region Not Implemented
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { throw new NotImplementedException(); }
        #endregion Not Implemented
    }
}

using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace MediaControlsLibrary.Converters
{
    public class NavigationViewSettingsConverter : IValueConverter
    {
        // Converter
        // ====================================================
        // ====================================================
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Convert Value to Boolean
            bool.TryParse($"{value}", out bool result);

            //Check if result is Set to True
            if(result == true)
            {
                //Return Visibility
                return Visibility.Visible;
            }
            else
            {
                //Return Visibility
                return Visibility.Collapsed;
            }
        }


        #region Not Implemented
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion Not Implemented
    }
}
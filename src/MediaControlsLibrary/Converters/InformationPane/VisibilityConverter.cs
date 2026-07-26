using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace MediaControlsLibrary.Converters
{
    class VisibilityConverter : IValueConverter
    {
        // Converter
        // ====================================================
        // ====================================================
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Check if Visibility is Set to Visible
            if((Visibility)value == Visibility.Visible)
            {
                //Return Visibility Collapsed
                return Visibility.Collapsed;
            }
            else
            {
                //Return Visibility Visible
                return Visibility.Visible;
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

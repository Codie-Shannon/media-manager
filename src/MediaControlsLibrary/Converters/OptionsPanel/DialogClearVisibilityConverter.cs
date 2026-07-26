using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MediaControlsLibrary.Converters
{
    public class DialogClearVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Convert value to Boolean
            bool.TryParse(value.ToString(), out bool isvisible);

            //Get and Return Visibility
            return isvisible ? Visibility.Visible : Visibility.Collapsed;
        }


        #region Not Implemented
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { throw new NotImplementedException(); }
        #endregion Not Implemented
    }
}
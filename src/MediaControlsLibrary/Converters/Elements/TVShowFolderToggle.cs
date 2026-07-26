using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MediaControlsLibrary.Converters.Elements
{
    public class TVShowFolderToggle : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Convert Object to Boolean
            bool.TryParse(value.ToString(), out bool istvshow);

            //Validate and Return Visibility Value
            return istvshow ? Visibility.Collapsed : Visibility.Visible;
        }


        #region Not Implemented
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { throw new NotImplementedException(); }
        #endregion Not Implemented
    }
}

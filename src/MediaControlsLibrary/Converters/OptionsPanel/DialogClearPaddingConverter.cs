using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MediaControlsLibrary.Converters
{
    public class DialogClearPaddingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Convert value to Boolean
            bool.TryParse(value.ToString(), out bool isclear);

            //Get and Return Padding
            return isclear ? new Thickness(6, 0, 55, 0) : new Thickness(6, 0, 6, 0);
        }


        #region Not Implemented
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { throw new NotImplementedException(); }
        #endregion Not Implemented
    }
}
using System;
using System.Windows.Data;
using System.Globalization;

namespace MediaControlsLibrary.Converters
{
    class VacantConverter : IMultiValueConverter, IValueConverter
    {
        // Converters
        // ====================================================
        // ====================================================
        public object Convert(object[] vals, Type targetType, object parameter, CultureInfo culture)
        {
            //Check if values 0 and 1 of the array are equal to 0
            if(((float)vals[0]) == 0 && ((int)vals[1]) == 0)
            {
                //Set Text to Vacant
                return Properties.Settings.Default.Vacant;
            }

            //Return String Empty
            return string.Empty;
        }

        public object Convert(object val, Type targetType, object parameter, CultureInfo culture)
        {
            //Check if the value is equal to an empty or null string
            if (string.IsNullOrEmpty((string)val))
            {
                //Set Text to Vacant
                return Properties.Settings.Default.Vacant;
            }

            //Return Original Value
            return val;
        }


        #region Not Implemented
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion Not Implemented
    }
}
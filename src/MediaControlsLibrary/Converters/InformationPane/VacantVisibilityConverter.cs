using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace MediaControlsLibrary.Converters
{
    class VacantVisibilityConverter : IMultiValueConverter, IValueConverter
    {
        // Converters
        // ====================================================
        // ====================================================
        public object Convert(object[] vals, Type targetType, object parameter, CultureInfo culture)
        {
            //Check if values 0 and 1 of the array are equal to 0 and if the global variable of vacant visibility is set to false
            if (((float)vals[0]) == 0 && ((int)vals[1]) == 0 && Properties.Settings.Default.VacantVisibility == false)
            {
                //Return Visibility Collasped
                return Visibility.Collapsed;
            }

            //Return Visilbity Visible
            return Visibility.Visible;
        }

        public object Convert(object val, Type targetType, object parameter, CultureInfo culture)
        {
            //Get Value Type
            string type = ((string)parameter).ToLower();

            //Check if the value type is equal to generic and if the value is equal to null or empty
            //Or check if the value type is rating and if the value is set to Visibility.Collapsed
            if ((type == "generic" && string.IsNullOrEmpty((string)val)) || (type == "rating" && (Visibility)val == Visibility.Collapsed))
            {
                //Check if the Global Variable of Vacant Visibility is Set to False
                if (Properties.Settings.Default.VacantVisibility == false)
                {
                    //Return Visibility Collasped
                    return Visibility.Collapsed;
                }
            }

            //Return Visilbity Visible
            return Visibility.Visible;
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
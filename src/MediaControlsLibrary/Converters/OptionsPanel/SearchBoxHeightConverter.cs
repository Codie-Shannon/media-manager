using System;
using System.Windows.Data;
using System.Globalization;

namespace MediaControlsLibrary.Converters
{
    public class SearchBoxHeightConverter : IMultiValueConverter
    {
        // Converter
        // ====================================================
        // ====================================================
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            //Get Parsed Values
            double.TryParse(values[0].ToString(), out double searchboxheight);
            double.TryParse(values[1].ToString(), out double contentheight);

            //Return Calculated Height
            return searchboxheight - (contentheight * (contentheight > 40 ? 1.63 : 3.3));
        }


        #region Not Implemented
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) { throw new NotImplementedException(); }
        #endregion Not Implemented
    }
}
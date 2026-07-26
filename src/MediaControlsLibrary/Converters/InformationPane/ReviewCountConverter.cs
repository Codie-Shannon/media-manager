using System;
using System.Windows.Data;
using System.Globalization;

namespace MediaControlsLibrary.Converters
{
    class ReviewCountConverter : IValueConverter
    {
        // Converter
        // ====================================================
        // ====================================================
        public object Convert(object val, Type targetType, object parameter, CultureInfo culture)
        {
            //Get Review Count
            float.TryParse(val.ToString(), out float result);

            //Format Review Count
            string reviewCount = String.Format("{0:n0}", result);

            //Return Review Count
            return reviewCount;
        }


        #region Not Implemented
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion Not Implemented
    }
}

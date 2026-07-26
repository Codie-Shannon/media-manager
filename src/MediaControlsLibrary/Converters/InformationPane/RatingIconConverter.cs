using System;
using System.Windows.Data;
using System.Globalization;
using static MediaControlsLibrary.Types.Icons;

namespace MediaControlsLibrary.Converters
{
    public class RatingIconConverter : IValueConverter
    {
        // Converter
        // ====================================================
        // ====================================================
        public object Convert(object val, Type targetType, object parameter, CultureInfo culture)
        {
            //Convert Object to Rating Type
            RatingType value = (RatingType)val;

            //Get The Set Icon's Value
            RatingIcons.TryGetValue(value, out string result);

            //Return The Set Icon's Value
            return result;
        }


        #region Not Implemented
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion Not Implemented
    }
}
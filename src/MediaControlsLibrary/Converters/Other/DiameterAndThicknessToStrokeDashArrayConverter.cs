using System;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;

namespace MediaControlsLibrary.Converters
{
    public class DiameterAndThicknessToStrokeDashArrayConverter : IMultiValueConverter
    {
        // Converter
        // ====================================================
        // ====================================================
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            //Check if the values array does not contain 2 or more values, and check if the first and second value of the values array cannot be converted to doubles
            if (values.Length < 2 || !double.TryParse(values[0].ToString(), out double diameter) || !double.TryParse(values[1].ToString(), out double thickness))
            {
                //Return 0
                return 0;
            }

            //Calculate circumference, lineLength, and gapLength
            double circumference = Math.PI * diameter;
            double lineLength = circumference * 0.75;
            double gapLength = circumference - lineLength;

            //Divide lineLength and gapLength by thickness and return them
            return new DoubleCollection(new[] { lineLength / thickness, gapLength / thickness });
        }


        #region Not Implemented
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion Not Implemented
    }
}
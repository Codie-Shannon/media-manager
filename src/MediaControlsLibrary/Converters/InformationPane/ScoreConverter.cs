using System;
using System.Windows.Data;
using System.Globalization;

namespace MediaControlsLibrary.Converters
{
    class ScoreConverter : IMultiValueConverter
    {
        // Converter
        // ====================================================
        // ====================================================
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            //Variables
            string str_score = "";

            //Get Values
            float.TryParse(values[0].ToString(), out float score);
            float.TryParse(values[1].ToString(), out float scoredivison);
            bool isCritic = (bool)values[2];

            //Check if the entered score is for critics
            if (isCritic == true)
            {
                //Calculate and Format Score
                str_score = (score / scoredivison).ToString("0.0");
            }

            //Get Score Value
            str_score = isCritic ? str_score : score.ToString();

            //Return Score
            return $"{str_score} / 10";
        }


        #region Not Implemented
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion Not Implemented
    }
}

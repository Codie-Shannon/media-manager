using System;
using System.Windows.Data;
using System.Globalization;

namespace MediaControlsLibrary.Converters.FolderBrowser
{
    public class fbSelectedFolderConverter : IMultiValueConverter
    {
        // Converter
        // ====================================================
        // ====================================================
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            //Variables
            int selectedId = 0, id = 0;

            //Check if the values array contains two elements 
            if (values.Length == 2)
            {
                //Convert Values to Integers
                int.TryParse(values[0].ToString(), out selectedId);
                int.TryParse(values[1].ToString(), out id);

                //Validate Id
                if (selectedId == id)
                {
                    //Return True
                    return true;
                }
            }

            //Return False
            return false;
        }


        #region Not Implemented
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion Not Implemented
    }
}
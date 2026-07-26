using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MediaControlsLibrary
{
    public class ipProperty : HeaderedContentControl
    {
        // Properties
        // ====================================================
        // ====================================================
        public new string Content
        {
            get => (string)GetValue(ContentProperty);
            set { ClearValue(ContentProperty); SetValue(ContentProperty, value); }
        }


        // Constructor
        // ====================================================
        // ====================================================
        static ipProperty()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ipProperty), new FrameworkPropertyMetadata(typeof(ipProperty)));
        }


        // Methods
        // ====================================================
        // ====================================================
        public void SetValue(string value) { this.Content = value; }

        public void SetList(List<string> values, bool isSort = true)
        {
            //Variables
            string value = "";

            //Check if the isSort boolean is set to true
            if (isSort)
            {
                //Sort List
                values.Sort();
            }

            //Check if list count is above 0
            if (values.Count > 0)
            {
                //Loop through element in the list
                for (int i = 0; i < values.Count; i++)
                {
                    //Add current looped value to value string variable
                    value += values[i];

                    //Check if the current looped value is not the last value within the list
                    if (i != values.Count - 1)
                    {
                        value += "\n";
                    }
                }

                //Set Formatted Values
                Content = value;
            }
            else
            {
                //Set Empty String
                Content = string.Empty;
            }
        }

        public void Clear() { this.Content = string.Empty; }
    }
}
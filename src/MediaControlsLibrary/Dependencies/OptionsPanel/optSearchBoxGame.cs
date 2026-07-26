using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using MediaControlsLibrary.Dependencies;

namespace MediaControlsLibrary
{
    public class optSearchBoxGame : SearchBoxItemBase
    {
        // Variables
        // ====================================================
        // ====================================================
        // ====================================================
        private const string str_Platforms = "PART_Platforms";
        private TextBlock PART_Platforms { get; set; }



        // Fields
        // ====================================================
        // ====================================================
        // ====================================================
        public static readonly DependencyProperty IGDBLinkProperty = DependencyProperty.Register(nameof(IGDBLink), typeof(string), typeof(optSearchBoxGame), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(nameof(Type), typeof(string), typeof(optSearchBoxGame), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty PlatformsProperty = DependencyProperty.Register(nameof(Platforms), typeof(List<string>), typeof(optSearchBoxGame), new PropertyMetadata(default(List<string>)));



        #region Properties
        // IGDB Link
        // =========================================================
        // =========================================================
        public string IGDBLink
        {
            get => (string)GetValue(IGDBLinkProperty);
            set => SetValue(IGDBLinkProperty, value);
        }


        // Type
        // =========================================================
        // =========================================================
        public string Type
        {
            get => (string)GetValue(TypeProperty);
            set => SetValue(TypeProperty, value);
        }


        // Platforms
        // =========================================================
        // =========================================================
        public List<string> Platforms
        {
            get => (List<string>)GetValue(PlatformsProperty);
            set => SetValue(PlatformsProperty, value);
        }
        #endregion Properties



        // Constructor
        // ====================================================
        // ====================================================
        // ====================================================
        static optSearchBoxGame()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(optSearchBoxGame), new FrameworkPropertyMetadata(typeof(optSearchBoxGame)));
        }



        // Apply Template
        // =========================================================
        // =========================================================
        public override void OnApplyTemplate()
        {
            //Fire When the Element's Template is Applied
            base.OnApplyTemplate();

            //Get TextBlock Element
            PART_Platforms = (TextBlock)this.Template.FindName(str_Platforms, this);

            //Format and Display Platforms
            PART_Platforms.Text = SetPlatforms(Platforms);
        }



        // Extensions
        // =========================================================
        // =========================================================
        private string SetPlatforms(List<string> platforms)
        {
            //Variables
            string value = "";

            //Check if the Platforms List Contains Any Platforms
            if (platforms != null && platforms.Count > 0)
            {
                //Loop through Platforms List
                for (int i = 0; i < platforms.Count; i++)
                {
                    //Check if the Current Looped Platform Value is Not the Last Platform Value withint the Platforms List 
                    if (i < platforms.Count - 1)
                    {
                        //Add Current Looped Platform to value String
                        value += $"{platforms[i]}, ";
                    }
                    else
                    {
                        //Add Current Looped Platform to value String
                        value += platforms[i];
                    }
                }
            }
            else
            {
                //Collapse Platforms TextBlock
                PART_Platforms.Visibility = Visibility.Collapsed;
            }

            //Return Value
            return value;
        }
    }
}
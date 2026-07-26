using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MediaControlsLibrary.Dependencies
{
    public class SearchBoxItemBase : ContentControl
    {
        // Variables
        // =========================================================
        // =========================================================
        private const string str_Cover = "PART_Image";
        private static ImageBrush PART_Cover { get; set; }



        // Fields
        // ====================================================
        // ====================================================
        public static new readonly DependencyProperty NameProperty = DependencyProperty.Register(nameof(Name), typeof(string), typeof(SearchBoxItemBase), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty CoverImageProperty = DependencyProperty.Register(nameof(CoverImage), typeof(string), typeof(SearchBoxItemBase), new PropertyMetadata(default(string)));



        // Properties
        // =========================================================
        // =========================================================
        public new string Name
        {
            get => (string)GetValue(NameProperty);
            set => SetValue(NameProperty, value);
        }

        public string CoverImage
        {
            get => (string)GetValue(CoverImageProperty);
            set => SetValue(CoverImageProperty, value);
        }



        // Apply Template
        // =========================================================
        // =========================================================
        public override void OnApplyTemplate()
        {
            //Fire When the Element's Template is Applied
            base.OnApplyTemplate();

            //Get Element
            PART_Cover = (ImageBrush)Template.FindName(str_Cover, this);

            //Set Cover Image
            SetImage(CoverImage);
        }



        // Methods
        // =========================================================
        // =========================================================
        private void SetImage(string path)
        {
            //Validate Path
            if (!string.IsNullOrEmpty(path))
            {
                //Create Bitmap Image Object
                BitmapImage source = new BitmapImage();

                //Begin Initialization of Bitmap Image Object
                source.BeginInit();

                //Set Settings
                source.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                source.CacheOption = BitmapCacheOption.OnLoad;

                //Set UriSource
                source.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);

                //End Initialization
                source.EndInit();

                //Set Cover
                PART_Cover.ImageSource = source;
            }
        }
    }
}
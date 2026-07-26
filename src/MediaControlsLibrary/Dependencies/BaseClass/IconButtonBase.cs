using System.Windows;
using System.Windows.Controls;

namespace MediaControlsLibrary.Dependencies
{
    public class IconButtonBase : Button
    {
        #region Fields
        // Submenu and Options Panel
        // ====================================================
        // ====================================================
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(nameof(Icon), typeof(string), typeof(IconButtonBase), new PropertyMetadata(default(string)));
        public static new readonly DependencyProperty ContentProperty = DependencyProperty.Register(nameof(Content), typeof(string), typeof(IconButtonBase), new PropertyMetadata(default(string)));


        // Viewer
        // ====================================================
        // ====================================================
        public static readonly DependencyProperty PrimaryIconProperty = DependencyProperty.Register(nameof(PrimaryIcon), typeof(string), typeof(IconButtonBase), new PropertyMetadata(default(string), OnValueChanged));
        public static readonly DependencyProperty SecondaryIconProperty = DependencyProperty.Register(nameof(SecondaryIcon), typeof(string), typeof(IconButtonBase), new PropertyMetadata(default(string)));
        #endregion Fields



        #region Properties
        // Submenu and Options Panel
        // ====================================================
        // ====================================================
        public string Icon
        {
            get => (string)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public new string Content
        {
            get => (string)GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }


        // Viewer
        // ====================================================
        // ====================================================
        public string PrimaryIcon
        {
            get => (string)GetValue(PrimaryIconProperty);
            set => SetValue(PrimaryIconProperty, value);
        }

        public string SecondaryIcon
        {
            get => (string)GetValue(SecondaryIconProperty);
            set => SetValue(SecondaryIconProperty, value);
        }
        #endregion Properties



        // Event Handlers
        // ====================================================
        // ====================================================
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Convert Dependency Object to IconButtonBase Object
            IconButtonBase iconButton = d as IconButtonBase;

            //Set Icon to Primary Icon
            iconButton.Icon = iconButton.PrimaryIcon;
        }
    }
}
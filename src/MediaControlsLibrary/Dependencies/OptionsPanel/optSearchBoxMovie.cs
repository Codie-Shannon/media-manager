using System.Windows;
using MediaControlsLibrary.Dependencies;

namespace MediaControlsLibrary
{
    public class optSearchBoxMovie : SearchBoxItemBase
    {
        // Fields
        // ====================================================
        // ====================================================
        public static readonly DependencyProperty IMDBLinkProperty = DependencyProperty.Register(nameof(IMDBLink), typeof(string), typeof(optSearchBoxMovie), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty MetacriticLinkProperty = DependencyProperty.Register(nameof(MetacriticLink), typeof(string), typeof(optSearchBoxMovie), new PropertyMetadata(default(string)));



        // Properties
        // =========================================================
        // =========================================================
        public string IMDBLink
        {
            get => (string)GetValue(IMDBLinkProperty);
            set => SetValue(IMDBLinkProperty, value);
        }

        public string MetacriticLink
        {
            get => (string)GetValue(MetacriticLinkProperty);
            set => SetValue(MetacriticLinkProperty, value);
        }



        // Constructor
        // ====================================================
        // ====================================================
        static optSearchBoxMovie()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(optSearchBoxMovie), new FrameworkPropertyMetadata(typeof(optSearchBoxMovie)));
        }
    }
}
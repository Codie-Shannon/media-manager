using System.Windows;
using System.Windows.Controls;

namespace MediaControlsLibrary.Dependencies
{
    public class NavigationViewBase : ItemsControl
    {
        // Fields
        // =====================================================
        // =====================================================
        public static readonly DependencyProperty IsSettingsProperty = DependencyProperty.Register(nameof(IsSettings), typeof(bool), typeof(NavigationViewBase), new PropertyMetadata(default(bool)));


        // Properties
        // =====================================================
        // =====================================================
        public bool IsSettings
        {
            get => (bool)GetValue(IsSettingsProperty);
            set => SetValue(IsSettingsProperty, value);
        }
    }
}
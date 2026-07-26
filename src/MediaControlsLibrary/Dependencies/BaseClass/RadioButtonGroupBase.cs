using System.Windows;
using System.Windows.Controls;

namespace MediaControlsLibrary.Dependencies
{
    public class RadioButtonGroupBase : ItemsControl
    {
        // Fields
        // ====================================================
        // ====================================================
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(nameof(Header), typeof(string), typeof(RadioButtonGroupBase), new PropertyMetadata(default(string)));


        // Properties
        // ====================================================
        // ====================================================
        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }
    }
}

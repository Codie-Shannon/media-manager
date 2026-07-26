using System.Windows;
using System.Windows.Controls;

namespace MediaControlsLibrary.Dependencies
{
    public class RadioButtonBase : RadioButton
    {
        // Fields
        // =========================================
        // =========================================
        public static readonly DependencyProperty DBNameProperty = DependencyProperty.Register(nameof(DBName), typeof(string), typeof(RadioButtonBase), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty IsReverseProperty = DependencyProperty.Register(nameof(IsReverse), typeof(bool), typeof(RadioButtonBase), new PropertyMetadata(default(bool)));


        // Properties
        // =========================================
        // =========================================
        public string DBName
        {
            get => (string)GetValue(DBNameProperty);
            set => SetValue(DBNameProperty, value);
        }

        public bool IsReverse
        {
            get => (bool)GetValue(IsReverseProperty);
            set => SetValue(IsReverseProperty, value);
        }
    }
}
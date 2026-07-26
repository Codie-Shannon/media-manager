using System.Windows;
using System.Windows.Controls;

namespace MediaControlsLibrary.Dependencies
{
    public class CheckBoxBase : CheckBox
    {
        // Fields
        // ====================================================
        // ====================================================
        public static readonly DependencyProperty DBNameProperty = DependencyProperty.Register(nameof(DBName), typeof(string), typeof(CheckBoxBase), new PropertyMetadata(default(string)));


        // Properties
        // ====================================================
        // ====================================================
        public string DBName
        {
            get => (string)GetValue(DBNameProperty);
            set => SetValue(DBNameProperty, value);
        }
    }
}
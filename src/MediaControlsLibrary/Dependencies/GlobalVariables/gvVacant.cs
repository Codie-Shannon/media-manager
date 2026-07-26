using System.Windows;
using System.Windows.Controls;

namespace MediaControlsLibrary
{
    public class gvVacant : Control
    {
        // Fields
        // ====================================================
        // ====================================================
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(nameof(Message), typeof(string), typeof(gvVacant), new PropertyMetadata(default(string), OnValueChanged));


        // Properties
        // ====================================================
        // ====================================================
        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }


        // Constructor
        // ====================================================
        // ====================================================
        static gvVacant()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(gvVacant), new FrameworkPropertyMetadata(typeof(gvVacant)));
        }


        // Event Handlers
        // ====================================================
        // ====================================================
        private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            //Convert Dependency Object to gvVacant Object
            gvVacant _object = obj as gvVacant;

            //Set Vacant to Set Message Value
            Properties.Settings.Default.Vacant = _object.Message;
        }
    }
}

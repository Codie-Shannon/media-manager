using System.Windows;
using System.Windows.Controls;

namespace MediaControlsLibrary.Dependencies
{
    public class ComboBoxBase : ComboBox
    {
        #region Fields
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(nameof(Icon), typeof(string), typeof(ComboBoxBase), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(nameof(Content), typeof(string), typeof(ComboBoxBase), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty SelectedRadioProperty = DependencyProperty.Register(nameof(SelectedRadio), typeof(int), typeof(ComboBoxBase), new PropertyMetadata(0));
        public static readonly DependencyProperty SelectedCheckBoxProperty = DependencyProperty.Register(nameof(SelectedCheckBox), typeof(int), typeof(ComboBoxBase), new PropertyMetadata(0));
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(nameof(Type), typeof(string), typeof(ComboBoxBase), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty OrderProperty = DependencyProperty.Register(nameof(Order), typeof(string), typeof(ComboBoxBase), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty IsReverseProperty = DependencyProperty.Register(nameof(IsReverse), typeof(bool), typeof(ComboBoxBase), new PropertyMetadata(default(bool)));
        #endregion Fields



        #region Properties
        public string Icon
        {
            get => (string)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public string Content
        {
            get => (string)GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        public int SelectedRadio
        {
            get => (int)GetValue(SelectedRadioProperty);
            set => SetValue(SelectedRadioProperty, value);
        }

        public int SelectedCheckBox
        {
            get => (int)GetValue(SelectedCheckBoxProperty);
            set => SetValue(SelectedCheckBoxProperty, value);
        }

        public string Type
        {
            get => (string)GetValue(TypeProperty);
            set => SetValue(TypeProperty, value);
        }

        public string Order
        {
            get => (string)GetValue(OrderProperty);
            set => SetValue(OrderProperty, value);
        }

        public bool IsReverse
        {
            get => (bool)GetValue(IsReverseProperty);
            set => SetValue(IsReverseProperty, value);
        }
        #endregion Properties
    }
}

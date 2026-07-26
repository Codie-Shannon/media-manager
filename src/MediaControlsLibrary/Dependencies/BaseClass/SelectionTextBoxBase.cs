using System.Windows;
using System.Windows.Controls;

namespace MediaControlsLibrary.Dependencies
{
    public class SelectionTextBoxBase : TextBox
    {
        #region Fields
        public static new readonly DependencyProperty MaxLengthProperty = DependencyProperty.Register(nameof(MaxLength), typeof(int), typeof(SelectionTextBoxBase), new PropertyMetadata(default(int)));
        public static readonly DependencyProperty IdProperty = DependencyProperty.Register(nameof(Id), typeof(int), typeof(SelectionTextBoxBase), new PropertyMetadata(default(int)));
        public static readonly DependencyProperty DBNameProperty = DependencyProperty.Register(nameof(DBName), typeof(string), typeof(SelectionTextBoxBase), new PropertyMetadata(default(string)));
        #endregion Fields



        #region Properties
        public new int MaxLength
        {
            get => (int)GetValue(MaxLengthProperty);
            set => SetValue(MaxLengthProperty, value);
        }

        public int Id
        {
            get => (int)GetValue(IdProperty);
            set => SetValue(IdProperty, value);
        }

        public string DBName
        {
            get => (string)GetValue(DBNameProperty);
            set => SetValue(DBNameProperty, value);
        }
        #endregion Properties
    }
}
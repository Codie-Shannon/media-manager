using System;
using System.Windows;
using System.Windows.Controls;

namespace MediaControlsLibrary.Dependencies
{
    public class TextBoxBase : HeaderedContentControl
    {
        #region Variables
        // Element Variables
        // ====================================================
        // ====================================================
        private const string str_Content = "PART_Text";
        private TextBox tbContent { get; set; }


        // TextBox Event Handler Variable
        // ====================================================
        // ====================================================
        public event EventHandler<TextChangedEventArgs> TextChanged;
        #endregion Variables



        #region Fields
        public static readonly DependencyProperty LinkProperty = DependencyProperty.Register(nameof(Link), typeof(Uri), typeof(TextBoxBase), new PropertyMetadata(default(Uri)));
        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register(nameof(Placeholder), typeof(string), typeof(TextBoxBase), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty MaxLengthProperty = DependencyProperty.Register(nameof(MaxLength), typeof(int), typeof(TextBoxBase), new PropertyMetadata(int.MaxValue));
        #endregion Fields



        #region Properties
        public Uri Link
        {
            get => (Uri)GetValue(LinkProperty);
            set => SetValue(LinkProperty, value);
        }

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public int MaxLength
        {
            get => (int)GetValue(MaxLengthProperty);
            set => SetValue(MaxLengthProperty, value);
        }
        #endregion Properties



        // Event Handlers
        // ====================================================
        // ====================================================
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Set Content to TextBox Text
            this.Content = tbContent.Text;

            //Invoke Text Changed
            TextChanged?.Invoke(this, e);
        }



        // Methods
        // ====================================================
        // ====================================================
        public override void OnApplyTemplate()
        {
            //Fire When the Element's Template is Applied
            base.OnApplyTemplate();

            //Get TextBox Element
            tbContent = (TextBox)this.Template.FindName(str_Content, this);

            //Set Event Handlers
            tbContent.TextChanged += TextBox_TextChanged;
        }
    }
}
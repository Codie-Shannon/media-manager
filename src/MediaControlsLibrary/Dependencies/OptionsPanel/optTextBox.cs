using System.Windows;
using MediaControlsLibrary.Dependencies;

namespace MediaControlsLibrary
{
    public class optTextBox : TextBoxBase
    {
        static optTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(optTextBox), new FrameworkPropertyMetadata(typeof(optTextBox)));
        }
    }
}
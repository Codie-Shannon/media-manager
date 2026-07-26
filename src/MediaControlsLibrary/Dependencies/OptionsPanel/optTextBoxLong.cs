using System.Windows;
using MediaControlsLibrary.Dependencies;

namespace MediaControlsLibrary
{
    public class optTextBoxLong : TextBoxBase
    {
        static optTextBoxLong()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(optTextBoxLong), new FrameworkPropertyMetadata(typeof(optTextBoxLong)));
        }
    }
}
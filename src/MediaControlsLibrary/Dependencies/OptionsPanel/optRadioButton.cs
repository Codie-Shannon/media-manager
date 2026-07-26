using MediaControlsLibrary.Dependencies;
using System.Windows;

namespace MediaControlsLibrary
{
    public class optRadioButton : optRadioButtonBase
    {
        static optRadioButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(optRadioButton), new FrameworkPropertyMetadata(typeof(optRadioButton)));
        }
    }
}
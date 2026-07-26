using System.Windows;
using MediaControlsLibrary.Dependencies;

namespace MediaControlsLibrary
{
    public class subRadioButton : RadioButtonBase
    {
        static subRadioButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(subRadioButton), new FrameworkPropertyMetadata(typeof(subRadioButton)));
        }
    }
}
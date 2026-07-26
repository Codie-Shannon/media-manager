using System.Windows;
using MediaControlsLibrary.Dependencies;

namespace MediaControlsLibrary
{
    public class subButton : IconButtonBase
    {
        static subButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(subButton), new FrameworkPropertyMetadata(typeof(subButton)));
        }
    }
}
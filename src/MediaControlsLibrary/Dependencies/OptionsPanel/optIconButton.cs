using System.Windows;
using MediaControlsLibrary.Dependencies;

namespace MediaControlsLibrary
{
    public class optIconButton : IconButtonBase
    {
        static optIconButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(optIconButton), new FrameworkPropertyMetadata(typeof(optIconButton)));
        }
    }
}

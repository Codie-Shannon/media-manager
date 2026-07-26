using System.Windows;
using System.Windows.Controls;

namespace MediaControlsLibrary
{
    public class optButton : ContentControl
    {
        static optButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(optButton), new FrameworkPropertyMetadata(typeof(optButton)));
        }
    }
}
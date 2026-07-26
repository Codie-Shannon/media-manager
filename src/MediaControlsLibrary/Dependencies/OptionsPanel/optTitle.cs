using System.Windows;
using System.Windows.Controls;

namespace MediaControlsLibrary
{
    public class optTitle : ContentControl
    {
        static optTitle()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(optTitle), new FrameworkPropertyMetadata(typeof(optTitle)));
        }
    }
}
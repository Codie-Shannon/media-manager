using System.Windows;
using System.Windows.Controls;

namespace MediaControlsLibrary
{
    public class NavigationViewItemHeader : ContentControl
    {
        static NavigationViewItemHeader()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigationViewItemHeader), new FrameworkPropertyMetadata(typeof(NavigationViewItemHeader)));
        }
    }
}
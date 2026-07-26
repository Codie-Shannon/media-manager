using System.Windows;
using System.Windows.Controls;

namespace MediaControlsLibrary
{
    public class NavigationBarItemSeparator : Control
    {
        static NavigationBarItemSeparator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigationBarItemSeparator), new FrameworkPropertyMetadata(typeof(NavigationBarItemSeparator)));
        }
    }
}
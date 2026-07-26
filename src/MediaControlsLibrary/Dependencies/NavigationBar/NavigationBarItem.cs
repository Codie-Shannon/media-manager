using System.Windows;
using MediaControlsLibrary.Dependencies;

namespace MediaControlsLibrary
{
    public class NavigationBarItem : NavigationBarItemBase
    {
        static NavigationBarItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigationBarItem), new FrameworkPropertyMetadata(typeof(NavigationBarItem)));
        }
    }
}
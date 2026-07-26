using System.Windows;
using System.Windows.Controls;

namespace MediaControlsLibrary
{
    public class NavigationViewBack : Button
    {
        static NavigationViewBack()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigationViewBack), new FrameworkPropertyMetadata(typeof(NavigationViewBack)));
        }
    }
}
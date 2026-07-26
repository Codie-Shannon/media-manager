using System.Windows;
using System.Windows.Controls;

namespace MediaControlsLibrary
{
    public class subMenu : ItemsControl
    {
        static subMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(subMenu), new FrameworkPropertyMetadata(typeof(subMenu)));
        }
    }
}
using System.Windows;
using System.Windows.Controls;

namespace MediaControlsLibrary
{
    public class ContextMenuItem : MenuItem
    {
        static ContextMenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ContextMenuItem), new FrameworkPropertyMetadata(typeof(ContextMenuItem)));
        }
    }
}
using System.Windows;
using System.Windows.Controls;

namespace MediaControlsLibrary
{
    public class subGroupSeparator : Control
    {
        static subGroupSeparator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(subGroupSeparator), new FrameworkPropertyMetadata(typeof(subGroupSeparator)));
        }
    }
}
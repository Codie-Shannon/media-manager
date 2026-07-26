using System.Windows;
using System.Windows.Controls;

namespace MediaControlsLibrary
{
    public class optIconButtonGroup : ItemsControl
    {
        static optIconButtonGroup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(optIconButtonGroup), new FrameworkPropertyMetadata(typeof(optIconButtonGroup)));
        }
    }
}

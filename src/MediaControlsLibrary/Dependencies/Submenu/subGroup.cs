using System.Windows;
using MediaControlsLibrary.Dependencies;

namespace MediaControlsLibrary
{
    public class subGroup : SubmenuGroupBase
    {
        static subGroup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(subGroup), new FrameworkPropertyMetadata(typeof(subGroup)));
        }
    }
}
using System.Windows;
using MediaControlsLibrary.Dependencies;

namespace MediaControlsLibrary
{
    public class subCheckBox : CheckBoxBase
    {
        static subCheckBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(subCheckBox), new FrameworkPropertyMetadata(typeof(subCheckBox)));
        }
    }
}
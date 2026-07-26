using System.Windows;
using MediaControlsLibrary.Dependencies;

namespace MediaControlsLibrary
{
    public class optRadioButtonGroup : RadioButtonGroupBase
    {
        static optRadioButtonGroup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(optRadioButtonGroup), new FrameworkPropertyMetadata(typeof(optRadioButtonGroup)));
        }
    }
}
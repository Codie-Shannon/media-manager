using MediaControlsLibrary.Dependencies;
using System.Windows;

namespace MediaControlsLibrary
{
    public class optNumericBox : NumericBoxBase
    {
        static optNumericBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(optNumericBox), new FrameworkPropertyMetadata(typeof(optNumericBox)));
        }
    }
}
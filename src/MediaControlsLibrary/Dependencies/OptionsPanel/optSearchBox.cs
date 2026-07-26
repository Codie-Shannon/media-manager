using System.Windows;
using MediaControlsLibrary.Dependencies;

namespace MediaControlsLibrary
{
    public class optSearchBox : SearchBoxBase
    {
        static optSearchBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(optSearchBox), new FrameworkPropertyMetadata(typeof(optSearchBox)));
        }
    }
}
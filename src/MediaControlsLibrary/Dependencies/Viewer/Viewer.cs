using System.Windows;
using System.Windows.Controls;

namespace MediaControlsLibrary
{
    public class Viewer : Grid
    {
        static Viewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Viewer), new FrameworkPropertyMetadata(typeof(Viewer)));
        }
    }
}
using System.Windows;
using System.Windows.Controls;

namespace MediaControlsLibrary
{
    public class ipCover : Image
    {
        static ipCover()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ipCover), new FrameworkPropertyMetadata(typeof(ipCover)));
        }
    }
}
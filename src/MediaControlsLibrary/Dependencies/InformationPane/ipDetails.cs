using System.Windows;
using System.Windows.Controls;

namespace MediaControlsLibrary
{
    public class ipDetails : ItemsControl
    {
        static ipDetails()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ipDetails), new FrameworkPropertyMetadata(typeof(ipDetails)));
        }
    }
}
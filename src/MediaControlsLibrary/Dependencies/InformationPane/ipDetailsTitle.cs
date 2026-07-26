using System.Windows;
using System.Windows.Controls;

namespace MediaControlsLibrary
{
    public class ipDetailsTitle : ContentControl
    {
        static ipDetailsTitle()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ipDetailsTitle), new FrameworkPropertyMetadata(typeof(ipDetailsTitle)));
        }
    }
}
using System.Windows;
using System.Windows.Controls;

namespace MediaControlsLibrary
{
    public class ipTitle : ContentControl 
    {
        static ipTitle()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ipTitle), new FrameworkPropertyMetadata(typeof(ipTitle)));
        }
    }
}
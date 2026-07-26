using MediaControlsLibrary.Dependencies;
using System.Windows;

namespace MediaControlsLibrary
{
    public class Loading : LoadingBase
    {
        static Loading()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Loading), new FrameworkPropertyMetadata(typeof(Loading)));
        }
    }
}
using System.Windows;
using MediaControlsLibrary.Dependencies;

namespace MediaControlsLibrary
{
    public class CoverItem : ElementBase
    {
        static CoverItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CoverItem), new FrameworkPropertyMetadata(typeof(CoverItem)));
        }
    }
}
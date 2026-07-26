using System.Windows;
using MediaControlsLibrary.Dependencies;

namespace MediaControlsLibrary
{
    public class CoverFolder : FolderBase
    {
        static CoverFolder()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CoverFolder), new FrameworkPropertyMetadata(typeof(CoverFolder)));
        }
    }
}
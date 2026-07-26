using System.Windows;
using MediaControlsLibrary.Dependencies;

namespace MediaControlsLibrary
{
    public class ImageFolder : FolderBase
    {
        static ImageFolder()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageFolder), new FrameworkPropertyMetadata(typeof(ImageFolder)));
        }
    }
}
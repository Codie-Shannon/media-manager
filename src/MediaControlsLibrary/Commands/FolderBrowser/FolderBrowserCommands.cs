using System.Windows.Input;

namespace MediaControlsLibrary.Commands
{
    public class FolderBrowserCommands
    {
        public static RoutedCommand FolderClick = new RoutedCommand("FolderClick", typeof(FolderBrowserCommands));
    }
}
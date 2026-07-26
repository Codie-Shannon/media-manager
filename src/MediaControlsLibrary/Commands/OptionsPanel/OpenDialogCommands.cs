using System.Windows.Input;

namespace MediaControlsLibrary.Commands
{
    public class OpenDialogCommands
    {
        public static RoutedCommand RemoveClick = new RoutedCommand("RemoveClick", typeof(OpenDialogCommands));
    }
}
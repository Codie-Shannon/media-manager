using System;
using System.Windows;
using System.Collections.Generic;

namespace MediaControlsLibrary.Types
{
    public static class MessageBoxIcons
    {
        // Values
        // ======================================================
        // ======================================================
        public static Dictionary<MessageBoxImage, string> Icons = new Dictionary<MessageBoxImage, string>()
        {
            { MessageBoxImage.Error, "\uE783" },
            { MessageBoxImage.Exclamation, "\uE7BA" },
            { MessageBoxImage.Information, "\uF13F" },
            { MessageBoxImage.Question, "\uF142" }
        };
    }
}
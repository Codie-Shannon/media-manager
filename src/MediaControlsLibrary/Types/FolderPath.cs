using System;
using System.Collections.Generic;

namespace MediaControlsLibrary.Types
{
    public class FolderPath
    {
        // Labels
        // ======================================================
        // ======================================================
        public enum FolderPathType { Desktop, Documents, Music, Pictures, Videos };


        // Values
        // ======================================================
        // ======================================================
        public static Dictionary<FolderPathType, string> FolderPaths = new Dictionary<FolderPathType, string>()
        {
            { FolderPathType.Desktop, $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\" },
            { FolderPathType.Documents, $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\" },
            { FolderPathType.Music, $"{Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)}\\" },
            { FolderPathType.Pictures, $"{Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)}\\" },
            { FolderPathType.Videos, $"{Environment.GetFolderPath(Environment.SpecialFolder.MyVideos)}\\" }
        };
    }
}
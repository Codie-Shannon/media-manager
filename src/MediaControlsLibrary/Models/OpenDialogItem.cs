namespace MediaControlsLibrary.Models
{
    public class OpenDialogItem
    {
        // ID
        // ===============================================================
        // ===============================================================
        private int _id;

        public int Id { get => _id; set { _id = value; } }


        // Name
        // ===============================================================
        // ===============================================================
        private string _name;

        public string Name { get => _name; set { _name = value; } }


        // File Path
        // ===============================================================
        // ===============================================================
        private string _filePath;

        public string FilePath { get => _filePath; set { _filePath = value; } }


        // Cover
        // ===============================================================
        // ===============================================================
        private static string _cover;

        public static string Cover { get => _cover; set { _cover = value; } }


        // Remove Header
        // ===============================================================
        // ===============================================================
        private static string _removeHeader;

        public static string RemoveHeader { get => _removeHeader; set { _removeHeader = value; } }
    }
}
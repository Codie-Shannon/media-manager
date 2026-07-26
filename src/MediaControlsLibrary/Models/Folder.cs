namespace MediaControlsLibrary.Models
{
    public class Folder
    {
        // Database ID
        // ===============================================================
        // ===============================================================
        private int _id;

        public int Id { get => _id; set { _id = value; } }



        // Owner's Database ID
        // ===============================================================
        // ===============================================================
        private int _ownerId;

        public int OwnerId { get => _ownerId; set { _ownerId = value; } }



        // Name
        // ===============================================================
        // ===============================================================
        // Name
        private string _name;

        public string Name { get => _name; set { _name = value; } }


        // Custom Name
        private string _customName;

        public string CustomName { get => _customName; set { _customName = value; } }



        // Element Type (i.e. Movies, Videos, Pictures, Games etc)
        // ===============================================================
        // ===============================================================
        private string _type;

        public string Type { get => _type; set { _type = value; } }



        // Folder Type (i.e. Folders, TVShowFolders, SeasonFolders etc)
        // ===============================================================
        // ===============================================================
        private string _folderType;

        public string FolderType { get => _folderType; set { _folderType = value; } }



        // Is Favourite
        // ===============================================================
        // ===============================================================
        private int _isfavourite;

        public int isFavourite { get => _isfavourite; set { _isfavourite = value; } }



        // Cover Image
        // ===============================================================
        // ===============================================================
        // Cover Image
        private string _coverImage;

        public string CoverImage { get => _coverImage; set { _coverImage = value; } }


        // Custom Cover Image
        private string _customCoverImage;

        public string CustomCoverImage { get => _customCoverImage; set { _customCoverImage = value; } }



        // File Path
        // ===============================================================
        // ===============================================================
        private string _filePath;

        public string FilePath { get => _filePath; set { _filePath = value; } }
    }
}
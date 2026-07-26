using System.Collections.Generic;

namespace MediaControlsLibrary.Models
{
    public class GameSearch
    {
        // IGDB Link
        // ===============================================================
        // ===============================================================
        private string _igdbLink;

        public string IGDBLink { get => _igdbLink; set { _igdbLink = value; } }


        // Name
        // ===============================================================
        // ===============================================================
        private string _name;

        public string Name { get => _name; set { _name = value; } }


        // Cover Image
        // ===============================================================
        // ===============================================================
        private string _coverImage;

        public string CoverImage { get => _coverImage; set { _coverImage = value; } }


        // Type
        // ===============================================================
        // ===============================================================
        private string _type;

        public string Type { get => _type; set { _type = value; } }


        // Platforms
        // ===============================================================
        // ===============================================================
        private List<string> _platforms;

        public List<string> Platforms { get => _platforms; set { _platforms = value; } }
    }
}
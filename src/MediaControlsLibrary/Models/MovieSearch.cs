namespace MediaControlsLibrary.Models
{
    public class MovieSearch
    {
        // Name
        // ===============================================================
        // ===============================================================
        private string _name;

        public string Name { get => _name; set { _name = value; } }


        // Cover
        // ===============================================================
        // ===============================================================
        private string _coverImage;

        public string CoverImage { get => _coverImage; set { _coverImage = value; } }


        // IMDB Link
        // ===============================================================
        // ===============================================================
        private string _imdbLink;

        public string IMDBLink { get => _imdbLink; set { _imdbLink = value; } }


        // Metacritic Link
        // ===============================================================
        // ===============================================================
        private string _metacriticLink;

        public string MetacriticLink { get => _metacriticLink; set { _metacriticLink = value; } }
    }
}
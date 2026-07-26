using System.Collections.Generic;

namespace MediaControlsLibrary.Types
{
    public class Icons
    {
        #region Labels
        // Rating
        // ====================================================
        // ====================================================
        public enum RatingType { Star, StarOutline }


        // Volume
        // ====================================================
        // ====================================================
        public enum VolumeType { Mute, Bar_1, Bar_2, Bar_3 }
        #endregion Labels


        #region Values
        // Rating
        // ====================================================
        // ====================================================
        public static Dictionary<RatingType, string> RatingIcons = new Dictionary<RatingType, string>() 
        {
            { RatingType.Star, "\uE735" },
            { RatingType.StarOutline, "\uE734" }
        };


        // Volume
        // ====================================================
        // ====================================================
        public static Dictionary<VolumeType, string> VolumeIcons = new Dictionary<VolumeType, string>()
        {
            { VolumeType.Mute, "\uE74F" },
            { VolumeType.Bar_1, "\uE993" },
            { VolumeType.Bar_2, "\uE994" },
            { VolumeType.Bar_3, "\uE995" }
        };
        #endregion Values
    }
}

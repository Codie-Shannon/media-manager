using System.Windows;
using MediaControlsLibrary.Dependencies;
using static MediaControlsLibrary.Types.Icons;

namespace MediaControlsLibrary
{
    public class ipRating : RatingBase
    {
        // Constructor
        // ====================================================
        // ====================================================
        static ipRating()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ipRating), new FrameworkPropertyMetadata(typeof(ipRating)));
        }


        // Apply Template
        // ====================================================
        // ====================================================
        public override void OnApplyTemplate()
        {
            //Fire When the Element's Template is Applied
            base.OnApplyTemplate();

            //Set Rating
            SetRating(Score, ReviewCount);
        }


        #region Methods
        public void SetRating(float score, int reviewcount)
        {
            //Set Values
            Score = score;
            ReviewCount = reviewcount;

            //Check if score and review count variables are above 0
            if (score > 0 && reviewcount > 0)
            {
                //Set IsAvailable to Collapsed
                IsAvailable = Visibility.Collapsed;

                //Set IsAvailable to Visible
                IsAvailable = Visibility.Visible;
            }
            else
            {
                //Set IsAvailable to Visible
                IsAvailable = Visibility.Visible;

                //Set IsAvailable to Collapsed
                IsAvailable = Visibility.Collapsed;
            }

            //Switch Icons
            SwitchIcon();
        }

        public void Clear()
        {
            //Clear Values
            SetValue(ScoreProperty, 0);
            SetValue(ReviewCountProperty, 0);
        }

        private void SwitchIcon()
        {
            //Check if the Score variable is above 5 and check if the IsStarSwitch variable is set to true
            if (Score > 5 && IsStarSwitch)
            {
                //Set Star
                Icon = RatingType.Star;
            }
            else if (IsStarSwitch)
            {
                //Set Star to Outline
                Icon = RatingType.StarOutline;
            }
        }
        #endregion Methods
    }
}
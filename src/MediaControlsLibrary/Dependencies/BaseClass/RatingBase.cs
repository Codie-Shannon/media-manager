using System.Windows;
using System.Windows.Controls;
using static MediaControlsLibrary.Types.Icons;

namespace MediaControlsLibrary.Dependencies
{
    public class RatingBase : Control
    {
        #region Fields
        public static readonly DependencyProperty IsCriticProperty = DependencyProperty.Register(nameof(IsCritic), typeof(bool), typeof(RatingBase), new PropertyMetadata(default(bool)));
        public static readonly DependencyProperty IsStarSwitchProperty = DependencyProperty.Register(nameof(IsStarSwitch), typeof(bool), typeof(RatingBase), new PropertyMetadata(true));
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(nameof(Header), typeof(string), typeof(RatingBase), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(nameof(Icon), typeof(RatingType), typeof(RatingBase), new PropertyMetadata(default(RatingType)));
        public static readonly DependencyProperty ScoreProperty = DependencyProperty.Register(nameof(Score), typeof(float), typeof(RatingBase), new PropertyMetadata(default(float)));
        public static readonly DependencyProperty ScoreDivisonProperty = DependencyProperty.Register(nameof(ScoreDivison), typeof(float), typeof(RatingBase), new PropertyMetadata(10.0f));
        public static readonly DependencyProperty ReviewCountProperty = DependencyProperty.Register(nameof(ReviewCount), typeof(int), typeof(RatingBase), new PropertyMetadata(default(int)));
        public static readonly DependencyProperty IsAvailableProperty = DependencyProperty.Register(nameof(IsAvailable), typeof(Visibility), typeof(RatingBase), new PropertyMetadata(Visibility.Visible));
        #endregion Fields


        #region Properties
        public bool IsCritic
        {
            get => (bool)GetValue(IsCriticProperty);
            set => SetValue(IsCriticProperty, value);
        }

        public bool IsStarSwitch
        {
            get => (bool)GetValue(IsStarSwitchProperty);
            set => SetValue(IsStarSwitchProperty, value);
        }

        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public RatingType Icon
        {
            get => (RatingType)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public float Score
        {
            get => (float)GetValue(ScoreProperty);
            set => SetValue(ScoreProperty, value);
        }

        public float ScoreDivison
        {
            get => (float)GetValue(ScoreDivisonProperty);
            set => SetValue(ScoreDivisonProperty, value);
        }

        public int ReviewCount
        {
            get => (int)GetValue(ReviewCountProperty);
            set => SetValue(ReviewCountProperty, value);
        }

        public Visibility IsAvailable
        {
            get => (Visibility)GetValue(IsAvailableProperty);
            set => SetValue(IsAvailableProperty, value);
        }
        #endregion Properties
    }
}
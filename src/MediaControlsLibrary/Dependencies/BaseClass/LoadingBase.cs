using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MediaControlsLibrary.Dependencies
{
    public class LoadingBase : Control
    {
        #region Fields
        public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register(nameof(IsLoading), typeof(bool), typeof(LoadingBase), new PropertyMetadata(default(bool)));
        public static readonly DependencyProperty DiameterProperty = DependencyProperty.Register(nameof(Diameter), typeof(double), typeof(LoadingBase), new PropertyMetadata(100.0));
        public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register(nameof(Thickness), typeof(double), typeof(LoadingBase), new PropertyMetadata(4.0));
        public static new readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(nameof(Background), typeof(Brush), typeof(LoadingBase), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(40, 0, 0, 0))));
        public static new readonly DependencyProperty ForegroundProperty = DependencyProperty.Register(nameof(Foreground), typeof(Brush), typeof(LoadingBase), new PropertyMetadata(Brushes.Red));
        public static readonly DependencyProperty CapProperty = DependencyProperty.Register(nameof(Cap), typeof(PenLineCap), typeof(LoadingBase), new PropertyMetadata(PenLineCap.Round));
        #endregion Fields



        #region Properties
        public bool IsLoading
        {
            get => (bool)GetValue(IsLoadingProperty);
            set => SetValue(IsLoadingProperty, value);
        }

        public double Diameter
        {
            get => (double)GetValue(DiameterProperty);
            set => SetValue(DiameterProperty, value);
        }

        public double Thickness
        {
            get => (double)GetValue(ThicknessProperty);
            set => SetValue(ThicknessProperty, value);
        }

        public new Brush Background
        {
            get => (Brush)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

        public new Brush Foreground
        {
            get => (Brush)GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }

        public PenLineCap Cap
        {
            get => (PenLineCap)GetValue(CapProperty);
            set => SetValue(CapProperty, value);
        }
        #endregion Properties
    }
}

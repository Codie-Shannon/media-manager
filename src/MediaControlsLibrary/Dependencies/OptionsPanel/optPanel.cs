using System.Windows;
using System.Windows.Controls;

namespace MediaControlsLibrary
{
    public class optPanel : ItemsControl
    {
        // Fields
        // ====================================================
        // ====================================================
        public static readonly DependencyProperty PanelWidthProperty = DependencyProperty.Register(nameof(PanelWidth), typeof(double), typeof(optPanel), new PropertyMetadata(650.0));
        public static readonly DependencyProperty PanelHeightProperty = DependencyProperty.Register(nameof(PanelHeight), typeof(double), typeof(optPanel), new PropertyMetadata(double.NaN));


        // Properties
        // ====================================================
        // ====================================================
        public double PanelWidth
        {
            get => (double)GetValue(PanelWidthProperty);
            set => SetValue(PanelWidthProperty, value);
        }

        public double PanelHeight
        {
            get => (double)GetValue(PanelHeightProperty);
            set => SetValue(PanelHeightProperty, value);
        }


        // Constructor
        // ====================================================
        // ====================================================
        static optPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(optPanel), new FrameworkPropertyMetadata(typeof(optPanel)));
        }
    }
}
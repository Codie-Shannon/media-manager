using System.Windows;
using MediaControlsLibrary.Dependencies;
using System.Windows.Controls.Primitives;

namespace MediaControlsLibrary
{
    public class subToggleButton : IconButtonBase
    {
        // Content Wrapper Variables
        // ====================================================
        // ====================================================
        public const string ContentWrapper = "PART_Content";
        private ToggleButton ContentWrapperToggleButton { get; set; }


        // Fields
        // ====================================================
        // ====================================================
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(subToggleButton), new PropertyMetadata(default(bool), OnSelectionChanged));


        // Properties
        // ====================================================
        // ====================================================
        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }


        // Event Handlers
        // ====================================================
        // ====================================================
        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            //Set IsSelected variable to IsChecked Value
            IsSelected = ContentWrapperToggleButton.IsChecked.Value;

            //Set The Global Variable of Vacant Visibility to the IsSelected Value
            Properties.Settings.Default.VacantVisibility = IsSelected;
        }

        private static void OnSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Get Child Toggle Button
            ToggleButton togglebutton = (ToggleButton)((subToggleButton)d).GetTemplateChild(ContentWrapper);

            //Check if a value has been Set
            if (!string.IsNullOrEmpty($"{e.NewValue}"))
            {
                //Check Toggle Button
                togglebutton.IsChecked = (bool)e.NewValue;
            }
        }


        // Constructor
        // ====================================================
        // ====================================================
        static subToggleButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(subToggleButton), new FrameworkPropertyMetadata(typeof(subToggleButton)));
        }


        // Apply Template
        // ====================================================
        // ====================================================
        public override void OnApplyTemplate()
        {
            //Fire When the Element's Template is Applied
            base.OnApplyTemplate();

            //Get Element's Toggle Button
            ContentWrapperToggleButton = (ToggleButton)GetTemplateChild(ContentWrapper);

            //Set Event Handlers
            ContentWrapperToggleButton.Checked += ToggleButton_Checked;
            ContentWrapperToggleButton.Unchecked += ToggleButton_Checked;
        }
    }
}
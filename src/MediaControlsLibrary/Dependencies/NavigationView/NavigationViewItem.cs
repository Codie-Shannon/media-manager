using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace MediaControlsLibrary
{
    // Element Template
    // ========================================================
    // ========================================================
    [TemplatePart(Name = ContentWrapper, Type = typeof(Button))]


    // Visual State Templates
    // ========================================================
    // ========================================================
    [TemplateVisualState(GroupName = "NavigationViewItemStates", Name = MouseOverStateName)]
    [TemplateVisualState(GroupName = "NavigationViewItemStates", Name = SelectedStateName)]
    [TemplateVisualState(GroupName = "NavigationViewItemStates", Name = UnselectedStateName)]


    public class NavigationViewItem : Control
    {
        // Menu Item States
        // ========================================================
        // ========================================================
        public const string MouseOverStateName = "MouseOver";
        public const string SelectedStateName = "Selected";
        public const string UnselectedStateName = "Unselected";


        // Content Wrapper Variables
        // ========================================================
        // ========================================================
        public const string ContentWrapper = "PART_Content";
        private Button ContentWrapperButton { get; set; }


        #region Fields
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(NavigationViewItem), new PropertyMetadata(default(bool), OnSelectedChanged));
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(nameof(Icon), typeof(string), typeof(NavigationViewItem), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(nameof(Label), typeof(string), typeof(NavigationViewItem), new PropertyMetadata(default(string)));
        #endregion Fields


        #region Properties
        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public string Icon
        {
            get => (string)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }
        #endregion Properties


        // Constructor
        // ========================================================
        // ========================================================
        static NavigationViewItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigationViewItem), new FrameworkPropertyMetadata(typeof(NavigationViewItem)));
        }


        // Apply Template
        // ========================================================
        // ========================================================
        public override void OnApplyTemplate()
        {
            //Fire When the Element's Template is Applied
            base.OnApplyTemplate();

            //Get Element's Button
            ContentWrapperButton = (Button)GetTemplateChild(ContentWrapper);

            //Check if the Element is Selected
            if (IsSelected)
            {
                //Set Visual State to Selected
                SwitchState(SelectedStateName);
            }
            else
            {
                //Set IsSelected to False
                IsSelected = false;

                //Set Visual State to Unselected
                SwitchState(UnselectedStateName);
            }

            //Set Event Handlers
            ContentWrapperButton.MouseEnter += ContentWrapperButton_MouseEnter;
            ContentWrapperButton.MouseLeave += ContentWrapperButton_MouseLeave;
            ContentWrapperButton.MouseLeftButtonUp += ContentWrapperButton_MouseLeftButtonUp;
        }


        #region Event Handlers
        private void ContentWrapperButton_MouseEnter(object sender, MouseEventArgs e)
        {
            SwitchState(MouseOverStateName);
        }

        private void ContentWrapperButton_MouseLeave(object sender, MouseEventArgs e)
        {
            if (IsSelected == true)
            {
                SwitchState(SelectedStateName);
            }
            else
            {
                SwitchState(UnselectedStateName);
            }
        }

        private void ContentWrapperButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsSelected)
            {
                SwitchState(SelectedStateName);
            }
            else
            {
                SwitchState(UnselectedStateName);
            }
        }
        #endregion Event Handlers


        #region Methods
        private void SwitchState(string state)
        {
            //Check if the state Variable is Set to SelectedStateName or Check if the Element is Unselected
            if (state == SelectedStateName || IsSelected == false)
            {
                //Change Visual States
                VisualStateManager.GoToState(this, state, true);
            }
        }

        private static void OnSelectedChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            //Convert obj variable to NavigationViewItem
            NavigationViewItem item = (NavigationViewItem)obj;

            if (e.NewValue is bool newValue)
            {
                //Check if the newValue Boolean is True or False
                if (newValue == true)
                {
                    //Change Visual State to Selected
                    VisualStateManager.GoToState(item, SelectedStateName, true);
                }
                else if (newValue == false)
                {
                    //Change Visual State to Unselected
                    VisualStateManager.GoToState(item, UnselectedStateName, true);
                }
            }
        }
        #endregion Methods
    }
}

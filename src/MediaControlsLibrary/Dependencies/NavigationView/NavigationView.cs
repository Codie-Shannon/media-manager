using System.Windows;
using System.Windows.Controls;
using MediaControlsLibrary.Dependencies;

namespace MediaControlsLibrary
{
    public class NavigationView : NavigationViewBase
    {
        // Element Variables
        // ========================================================
        // ========================================================
        private const string str_Settings = "PART_Button";
        private Button btnSettings { get; set; }


        // Fields
        // ========================================================
        // ========================================================
        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(nameof(Click), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NavigationView));


        // Properties
        // ========================================================
        // ========================================================
        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }


        // Constructor
        // ========================================================
        // ========================================================
        static NavigationView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigationView), new FrameworkPropertyMetadata(typeof(NavigationView)));
        }


        // Apply Template
        // ========================================================
        // ========================================================
        public override void OnApplyTemplate()
        {
            //Fire When the Element's Template is Applied
            base.OnApplyTemplate();

            //Get Element's Settings Button
            btnSettings = (Button)GetTemplateChild(str_Settings);

            //Set Event Handlers
            btnSettings.Click += (sender, args) => RaiseClickEvent();
        }


        // Methods
        // ========================================================
        // ========================================================
        private void RaiseClickEvent()
        {
            //Initialize RoutedEventArgs Object
            RoutedEventArgs newEventArgs = new RoutedEventArgs(ClickEvent);

            //Raise RoutedEventArgs Object Event
            RaiseEvent(newEventArgs);
        }
    }
}
using System.Windows;
using System.Windows.Controls;

namespace MediaControlsLibrary
{
    public class viewBar : ItemsControl
    {
        // Fields
        // ====================================================
        // ====================================================
        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(viewBar));


        // Properties
        // ====================================================
        // ====================================================
        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }


        // Constructor
        // ====================================================
        // ====================================================
        static viewBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(viewBar), new FrameworkPropertyMetadata(typeof(viewBar)));
        }


        // Apply Template
        // ====================================================
        // ====================================================
        public override void OnApplyTemplate()
        {
            //Fire When the Element's Template is Applied
            base.OnApplyTemplate();

            //Get Element's Button
            Button button = (Button)GetTemplateChild("PART_Button");

            //Set Event Handlers
            button.Click += (sender, args) => RaiseClickEvent();
        }


        // Methods
        // ====================================================
        // ====================================================
        private void RaiseClickEvent()
        {
            //Initialize RoutedEventArgs Object
            RoutedEventArgs newEventArgs = new RoutedEventArgs(ClickEvent);

            //Raise RoutedEventArgs Object Event
            RaiseEvent(newEventArgs);
        }
    }
}
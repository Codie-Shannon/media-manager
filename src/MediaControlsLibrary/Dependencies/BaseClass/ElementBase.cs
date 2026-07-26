using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MediaControlsLibrary.Dependencies
{
    // Visual State Templates
    // ====================================================
    // ====================================================
    [TemplateVisualState(GroupName = "ElementStates", Name = SelectedStateName)]
    [TemplateVisualState(GroupName = "ElementStates", Name = UnselectedStateName)]


    public class ElementBase : Button
    {
        #region Variables
        // Border
        // =========================================================
        // =========================================================
        private const string str_Content = "PART_Content";
        private static Button PART_Content { get; set; }


        // Background
        // =========================================================
        // =========================================================
        private const string str_Background = "PART_Image";
        private static Image PART_Background { get; set; }


        // Element States
        // =========================================================
        // =========================================================
        public const string SelectedStateName = "Selected";
        public const string UnselectedStateName = "Unselected";
        #endregion Variables



        #region Fields
        public static readonly DependencyProperty IdProperty = DependencyProperty.Register(nameof(Id), typeof(int), typeof(ElementBase), new PropertyMetadata(default(int)));
        public static readonly DependencyProperty MNameProperty = DependencyProperty.Register(nameof(MName), typeof(string), typeof(ElementBase), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty MCustomNameProperty = DependencyProperty.Register(nameof(MCustomName), typeof(string), typeof(ElementBase), new PropertyMetadata(default(string)));
        public static new readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(nameof(Background), typeof(string), typeof(ElementBase), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(ElementBase), new PropertyMetadata(default(bool), OnSelectionChanged));
        #endregion Fields



        #region Properties
        public int Id
        {
            get => (int)GetValue(IdProperty);
            set => SetValue(IdProperty, value);
        }

        public string MName
        {
            get => (string)GetValue(MNameProperty);
            set => SetValue(MNameProperty, value);
        }

        public string MCustomName
        {
            get => (string)GetValue(MCustomNameProperty);
            set => SetValue(MCustomNameProperty, value);
        }

        public new string Background
        {
            get => (string)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }
        #endregion Properties



        // Constructor
        // =========================================================
        // =========================================================
        static ElementBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ElementBase), new FrameworkPropertyMetadata(typeof(ElementBase)));
        }



        // Apply Template
        // =========================================================
        // =========================================================
        public override void OnApplyTemplate()
        {
            //Fire When the Element's Template is Applied
            base.OnApplyTemplate();

            //Get Elements
            PART_Content = (Button)this.Template.FindName(str_Content, this);
            PART_Background = (Image)this.Template.FindName(str_Background, this);

            //Set Background Image
            SetImage(Background);

            //Set Event Handlers
            PART_Content.MouseEnter += Element_Entered;
            PART_Content.MouseLeave += Element_Leave;
        }



        #region Event Handlers
        private void Element_Entered(object sender, MouseEventArgs e)
        {
            //Check if IsSelected is Set to False
            if (!IsSelected)
            {
                //Change Visual State
                SwitchState(SelectedStateName);
            }
        }

        private void Element_Leave(object sender, MouseEventArgs e)
        {
            //Check if IsSelected is Set to False
            if (!IsSelected)
            {
                //Change Visual State
                SwitchState(UnselectedStateName);
            }
        }

        private static void OnSelectionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            //Convert obj Variable to ElementBase
            ElementBase element = (ElementBase)obj;

            //Check if IsSelected is Set to True
            if (element.IsSelected)
            {
                //Change Visual State
                VisualStateManager.GoToState(element, SelectedStateName, true);
            }
            else
            {
                //Change Visual State
                VisualStateManager.GoToState(element, UnselectedStateName, true);
            }
        }
        #endregion Event Handlers



        // Methods
        // =========================================================
        // =========================================================
        private void SetImage(string path)
        {
            //Create Bitmap Image Object
            BitmapImage source = new BitmapImage();

            //Begin Initialization of Bitmap Image Object
            source.BeginInit();

            //Set Settings
            source.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            source.CacheOption = BitmapCacheOption.OnLoad;

            //Set UriSource
            source.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);

            //End Initialization
            source.EndInit();

            //Set Background
            PART_Background.Source = source;
        }

        private void SwitchState(string state)
        {
            //Change Visual States
            VisualStateManager.GoToState(this, state, true);
        }
    }
}
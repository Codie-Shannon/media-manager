using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static MediaControlsLibrary.Types.Icons;

namespace MediaControlsLibrary
{
    // Element Template
    // ====================================================
    // ====================================================
    [TemplatePart(Name = ContentWrapper, Type = typeof(ComboBox))]


    // Visual State Templates
    // ====================================================
    // ====================================================
    [TemplateVisualState(GroupName = "TestComboBoxStates", Name = InactiveStateName)]
    [TemplateVisualState(GroupName = "TestComboBoxStates", Name = MouseOverStateName)]
    [TemplateVisualState(GroupName = "TestComboBoxStates", Name = ActiveStateName)]


    public class viewVolumeBar : ComboBox
    {
        #region Variables
        // Menu Item States
        // ====================================================
        // ====================================================
        public const string InactiveStateName = "Inactive";
        public const string MouseOverStateName = "MouseOver";
        public const string ActiveStateName = "Active";


        // Content Wrapper
        // ====================================================
        // ====================================================
        public const string ContentWrapper = "PART_Content";
        private ComboBox ContentWrapperComboBox { get; set; }


        // Button
        // ====================================================
        // ====================================================
        private const string str_Mute = "PART_Mute";
        private Button btnMute { get; set; }


        // Slider
        // ====================================================
        // ====================================================
        private const string str_Slider = "PART_Slider";
        private Slider Slider { get; set; }


        // Event Handler Variables
        // ====================================================
        // ====================================================
        public event EventHandler<RoutedEventArgs> MuteClick;
        public event EventHandler<RoutedPropertyChangedEventArgs<double>> ValueUpdated;
        #endregion Variables



        #region Fields
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(double), typeof(viewVolumeBar), new PropertyMetadata(0.5));
        public static readonly DependencyProperty IsMutedProperty = DependencyProperty.Register(nameof(IsMuted), typeof(bool), typeof(viewVolumeBar), new PropertyMetadata(default(bool)));
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(nameof(Icon), typeof(VolumeType), typeof(viewVolumeBar), new PropertyMetadata(default(VolumeType)));
        #endregion Fields



        #region Properties
        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public bool IsMuted
        {
            get => (bool)GetValue(IsMutedProperty);
            set => SetValue(IsMutedProperty, value);
        }

        public VolumeType Icon
        {
            get => (VolumeType)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }
        #endregion Properties



        // Constructor
        // ==============================================
        // ==============================================
        static viewVolumeBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(viewVolumeBar), new FrameworkPropertyMetadata(typeof(viewVolumeBar)));
        }



        // Apply Template
        // ==============================================
        // ==============================================
        public override void OnApplyTemplate()
        {
            //Fire When the Element's Template is Applied
            base.OnApplyTemplate();

            //Get Elements
            ContentWrapperComboBox = (ComboBox)GetTemplateChild(ContentWrapper);
            btnMute = (Button)this.Template.FindName(str_Mute, this);
            Slider = (Slider)this.Template.FindName(str_Slider, this);

            //Set Visual State to Inactive
            VisualStateManager.GoToState(this, InactiveStateName, false);

            //Set Slider Value
            Slider.Value = Value * 100;

            //Update Icon
            UpdateIcon();

            //Set Event Handlers
            ContentWrapperComboBox.MouseEnter += ComboBox_MouseEnter;
            ContentWrapperComboBox.MouseLeave += ComboBox_MouseLeave;
            ContentWrapperComboBox.DropDownOpened += ComboBox_DropDownOpened;
            ContentWrapperComboBox.DropDownClosed += ComboBox_DropDownClosed;
            btnMute.Click += Mute_Clicked;
            Slider.ValueChanged += Slider_ValueChanged;
        }



        #region Event Handlers
        private void ComboBox_MouseEnter(object sender, MouseEventArgs e)
        {
            //Set Visual State to MouseOver
            SwitchState(MouseOverStateName);
        }

        private void ComboBox_MouseLeave(object sender, MouseEventArgs e)
        {
            //Set Visual State to Inactive
            SwitchState(InactiveStateName);
        }

        private void ComboBox_DropDownOpened(object sender, EventArgs e)
        {
            //Set Visual State to Active
            SwitchState(ActiveStateName);
        }

        private void ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            //Set Visual State to Inactive
            SwitchState(InactiveStateName);
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //Update Icon
            UpdateIcon();

            //Invoke Value Updated
            ValueUpdated?.Invoke(this, e);
        }

        private void Mute_Clicked(object sender, RoutedEventArgs e)
        {
            //Run Mute Method
            Mute();

            //Invoke Mute Click
            MuteClick?.Invoke(this, e);
        }
        #endregion Event Handlers



        #region Methods
        private void SwitchState(string statename)
        {
            //Change Visual State
            VisualStateManager.GoToState(this, statename, false);
        }

        public void UpdateIcon()
        {
            //Set Value Property to the Current Slider Value
            Value = Slider.Value;

            //Check Value
            if (Value == 0)
            {
                //Set IsMuted to True
                IsMuted = true;

                //Set Icon to Mute
                Icon = VolumeType.Mute;

                //Return Method
                return;
            }
            else if(Value > 0 && Value < 33)
            {
                //Set Icon to 1 Bar
                Icon = VolumeType.Bar_1;
            }
            else if(Value > 32 && Value < 66)
            {
                //Set Icon to 2 Bar
                Icon = VolumeType.Bar_2;
            }
            else if(Value > 65)
            {
                //Set Icon to 3 Bar
                Icon = VolumeType.Bar_3;
            }

            //Divide Value by 100
            Value = Value / 100;

            //Set IsMuted to False
            IsMuted = false;
        }

        public void Mute()
        {
            //Check if IsMuted is Set to True
            if (IsMuted)
            {
                //Set IsMuted to False
                IsMuted = false;

                //Update Icon
                UpdateIcon();
            }
            else
            {
                //Set IsMuted to True
                IsMuted = true;

                //Update Icon
                Icon = VolumeType.Mute;
            }
        }
        #endregion Methods
    }
}
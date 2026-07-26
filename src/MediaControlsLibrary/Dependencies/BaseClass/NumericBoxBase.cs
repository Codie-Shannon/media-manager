using System;
using System.Windows;
using System.Windows.Controls;

namespace MediaControlsLibrary.Dependencies
{
    public class NumericBoxBase : HeaderedContentControl
    {
        #region Variables
        // Increment
        // ====================================================
        // ====================================================
        private const string str_Increment = "PART_Increment";
        private Button PART_Increment { get; set; }


        // Decrement
        // ====================================================
        // ====================================================
        private const string str_Decrement = "PART_Decrement";
        private Button PART_Decrement { get; set; }


        // Behavioral
        // ====================================================
        // ====================================================
        public event EventHandler<RoutedEventArgs> Click;
        private int _value;
        #endregion Variables




        #region Fields
        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register(nameof(Placeholder), typeof(string), typeof(NumericBoxBase), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty MaxProperty = DependencyProperty.Register(nameof(Max), typeof(int), typeof(NumericBoxBase), new PropertyMetadata(int.MaxValue));
        public static readonly DependencyProperty MinProperty = DependencyProperty.Register(nameof(Min), typeof(int), typeof(NumericBoxBase), new PropertyMetadata(1));
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(int), typeof(NumericBoxBase), new PropertyMetadata(1));
        #endregion Fields




        #region Properties
        // Placeholder
        // ====================================================
        // ====================================================
        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }


        // Max
        // ====================================================
        // ====================================================
        public int Max
        {
            get => (int)GetValue(MaxProperty);
            set => SetValue(MaxProperty, value);
        }


        // Min
        // ====================================================
        // ====================================================
        public int Min
        {
            get => (int)GetValue(MinProperty);
            set => SetValue(MinProperty, value);
        }


        // Value
        // ====================================================
        // ====================================================
        public int Value
        {
            get => (int)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        #endregion Properties




        // On Apply Template
        // ====================================================
        // ====================================================
        // ====================================================
        public override void OnApplyTemplate()
        {
            //Fire When the Element's Template is Applied
            base.OnApplyTemplate();

            //Get Elements
            PART_Increment = (Button)this.Template.FindName(str_Increment, this);
            PART_Decrement = (Button)this.Template.FindName(str_Decrement, this);

            //Set Event Handlers
            this.Loaded += NumericBox_Loaded;
            PART_Increment.Click += Increment_Click;
            PART_Decrement.Click += Decrement_Click;

            //Set Value to Min
            _value = Min;
            Value = Min;

            //Set Content to Value
            Content = $"{Value}";

            //Set Max Value
            Max = Max >= Min ? Max : int.MaxValue;

            //Disable Decrement Button
            PART_Decrement.IsEnabled = false;
        }




        #region Event Handlers
        // Numeric Box
        // ====================================================
        // ====================================================
        private void NumericBox_Loaded(object sender, RoutedEventArgs e)
        {
            //Set Value
            SetValue(_value);
        }


        // Increment
        // ====================================================
        // ====================================================
        private void Increment_Click(object sender, RoutedEventArgs e)
        {
            //Validate Increment
            if(Value < Max)
            {
                //Increment Value
                Value++;

                //Set Content to Value
                Content = $"{Value}";

                //Toggle Increment Button
                PART_Increment.IsEnabled = Value == Max ? false : true;

                //Enable Decrement Button
                PART_Decrement.IsEnabled = true;

                //Invoke Click Event Handler
                Click?.Invoke(sender, e);
            }
        }


        // Decrement
        // ====================================================
        // ====================================================
        private void Decrement_Click(object sender, RoutedEventArgs e)
        {
            //Validate Decrement
            if(Value > Min)
            {
                //Increment Value
                Value--;

                //Set Content to Value
                Content = $"{Value}";

                //Enable Increment Button
                PART_Increment.IsEnabled = true;

                //Toggle Decrement Button
                PART_Decrement.IsEnabled = Value == Min ? false : true;

                //Invoke Click Event Handler
                Click?.Invoke(sender, e);
            }
        }
        #endregion Event Handlers




        #region Methods
        // Clear
        // ====================================================
        // ====================================================
        public void Clear()
        {
            //Set Value to Min Value
            Value = Min;

            //Set Content to Value
            Content = $"{Value}";
        }


        // Set Value
        // ====================================================
        // ====================================================
        public void SetValue(int value)
        {
            //Validate Load
            if (this.IsLoaded && PART_Decrement != null && PART_Increment != null)
            {
                //Set Value
                Value = value;

                //Set Content to Value
                Content = $"{Value}";

                //Toggle Decrement Button
                PART_Decrement.IsEnabled = Value == Min ? false : true;

                //Toggle Increment Button
                PART_Increment.IsEnabled = Value == Max ? false : true;
            }
            else
            {
                //Set Value
                _value = value;
            }
        }
        #endregion Methods
    }
}
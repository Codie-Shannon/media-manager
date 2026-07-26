using System.Windows;
using System.Windows.Controls;
using MediaControlsLibrary.Types;
using System.Collections.Generic;

namespace MediaControlsLibrary
{
    public class CustomMessageBox : Window
    {
        #region Variables
        // Caption Variables
        // ========================================================
        // ========================================================
        private string Caption { get { return Title; } set { Title = value; } }


        // Message Variables
        // ========================================================
        // ========================================================
        private string str_Icon = "PART_Icon";
        private TextBlock PART_Icon { get; set; }
        private string MessageBoxIcon { get; set; }


        // Message Variables
        // ========================================================
        // ========================================================
        private string str_Message = "PART_Message";
        private TextBlock PART_Message { get; set; }
        private string Message { get; set; }


        // OK Variables
        // ========================================================
        // ========================================================
        private string str_OK = "PART_OK";
        private Button PART_OK { get; set; }
        private string OK { get; set; }
        private string Default_OK = "OK";


        // Yes Variables
        // ========================================================
        // ========================================================
        private string str_Yes = "PART_Yes";
        private Button PART_Yes { get; set; }
        private string Yes { get; set; }
        private string Default_Yes = "Yes";


        // No Variables
        // ========================================================
        // ========================================================
        private string str_No = "PART_No";
        private Button PART_No { get; set; }
        private string No { get; set; }
        private string Default_No = "No";


        // Cancel Variables
        // ========================================================
        // ========================================================
        private string str_Cancel = "PART_Cancel";
        private Button PART_Cancel { get; set; }
        private string Cancel { get; set; }
        private string Default_Cancel = "Cancel";


        // Result Variables
        // ========================================================
        // ========================================================
        public MessageBoxResult Result;

        public Dictionary<string, MessageBoxResult> Results = new Dictionary<string, MessageBoxResult>()
        {
            { "ok", MessageBoxResult.OK },
            { "cancel", MessageBoxResult.Cancel },
            { "yes", MessageBoxResult.Yes },
            { "no", MessageBoxResult.No }
        };


        // Other Variables
        // ========================================================
        // ========================================================
        private MessageBoxButton ButtonType;
        private static bool isInitialCreation;
        #endregion Variables



        // Constructor
        // ========================================================
        // ========================================================
        public CustomMessageBox()
        {
            //Check if the window has not been opened before
            if (!isInitialCreation)
            {
                //Override Metadata
                DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomMessageBox), new FrameworkPropertyMetadata(typeof(CustomMessageBox)));

                //Set isInitialCreation to True
                isInitialCreation = true;
            }

            //Set Owner
            this.Owner = Application.Current.MainWindow;
        }



        // Setup
        // ========================================================
        // ========================================================
        private void SetStartupLocation()
        {
            //Set Window Startup Location to Manual
            this.WindowStartupLocation = WindowStartupLocation.Manual;

            //Calculate and Set Window Position to Center of Owner
            this.Top = (this.Owner.ActualHeight - this.ActualHeight) / 2;
            this.Left = (this.Owner.ActualWidth - this.ActualWidth) / 2;
        }

        private void SetupElements()
        {
            //Get Elements
            PART_Icon = (TextBlock)GetTemplateChild(str_Icon);
            PART_Message = (TextBlock)GetTemplateChild(str_Message);
            PART_OK = (Button)GetTemplateChild(str_OK);
            PART_Cancel = (Button)GetTemplateChild(str_Cancel);
            PART_Yes = (Button)GetTemplateChild(str_Yes);
            PART_No = (Button)GetTemplateChild(str_No);

            //Set Message
            PART_Message.Text = Message;

            //Set Button Values
            SetButtonText(PART_OK, Default_OK, OK);
            SetButtonText(PART_Yes, Default_Yes, Yes);
            SetButtonText(PART_No, Default_No, No);
            SetButtonText(PART_Cancel, Default_Cancel, Cancel);

            //Set Icon
            PART_Icon.Visibility = string.IsNullOrEmpty(MessageBoxIcon) ? Visibility.Collapsed : Visibility.Visible;
            PART_Icon.Text = MessageBoxIcon;

            //Display Buttons
            DisplayButtons(ButtonType);

            //Set Event Handlers
            PART_OK.Click += Button_Click;
            PART_Cancel.Click += Button_Click;
            PART_Yes.Click += Button_Click;
            PART_No.Click += Button_Click;
        }



        // Apply Template
        // ========================================================
        // ========================================================
        public override void OnApplyTemplate()
        {
            //Fire When the Element's Template is Applied
            base.OnApplyTemplate();

            //Set Startup Location
            SetStartupLocation();

            //Setup Elements
            SetupElements();
        }



        #region Show Dialog
        #region Generic
        public static MessageBoxResult ShowDialog(string message)
        {
            //Create MessageBox Object
            CustomMessageBox messagebox = new CustomMessageBox();

            //Set Values
            messagebox.SetValues(message);

            //Show MessageBox
            messagebox.ShowDialog();

            //Return Result
            return messagebox.Result;
        }

        public static MessageBoxResult ShowDialog(string message, string caption)
        {
            //Create MessageBox Object
            CustomMessageBox messagebox = new CustomMessageBox();

            //Set Values
            messagebox.SetValues(message, caption);

            //Show MessageBox
            messagebox.ShowDialog();

            //Return Result
            return messagebox.Result;
        }

        public static MessageBoxResult ShowDialog(string message, string caption, MessageBoxButton button)
        {
            //Create MessageBox Object
            CustomMessageBox messagebox = new CustomMessageBox();

            //Set Values
            messagebox.SetValues(message, caption, button);

            //Show MessageBox
            messagebox.ShowDialog();

            //Return Result
            return messagebox.Result;
        }

        public static MessageBoxResult ShowDialog(string message, string caption, MessageBoxButton button, MessageBoxImage icon)
        {
            //Create MessageBox Object
            CustomMessageBox messagebox = new CustomMessageBox();

            //Set Values
            messagebox.SetValues(message, caption, button, icon);

            //Show MessageBox
            messagebox.ShowDialog();

            //Return Result
            return messagebox.Result;
        }
        #endregion Generic


        #region Custom
        // Show OK
        // ========================================================
        // ========================================================
        public static MessageBoxResult ShowOK(string message, string caption, string ok_button_text)
        {
            //Create MessageBox Object
            CustomMessageBox messagebox = new CustomMessageBox();

            //Set Values
            messagebox.SetValues(message, caption, MessageBoxButton.OK, MessageBoxImage.None, ok_button_text);

            //Show MessageBox
            messagebox.ShowDialog();

            //Return Result
            return messagebox.Result;
        }

        public static MessageBoxResult ShowOK(string message, string caption, string ok_button_text, MessageBoxImage icon)
        {
            //Create MessageBox Object
            CustomMessageBox messagebox = new CustomMessageBox();

            //Set Values
            messagebox.SetValues(message, caption, MessageBoxButton.OK, icon, ok_button_text);

            //Show MessageBox
            messagebox.ShowDialog();

            //Return Result
            return messagebox.Result;
        }


        // Show OK / Cancel
        // ========================================================
        // ========================================================
        public static MessageBoxResult ShowOKCancel(string message, string caption, string ok_button_text, string cancel_button_text)
        {
            //Create MessageBox Object
            CustomMessageBox messagebox = new CustomMessageBox();

            //Set Values
            messagebox.SetValues(message, caption, MessageBoxButton.OKCancel, MessageBoxImage.None, ok_button_text, string.Empty, string.Empty, cancel_button_text);

            //Show MessageBox
            messagebox.ShowDialog();

            //Return Result
            return messagebox.Result;
        }

        public static MessageBoxResult ShowOKCancel(string message, string caption, string ok_button_text, string cancel_button_text, MessageBoxImage icon)
        {
            //Create MessageBox Object
            CustomMessageBox messagebox = new CustomMessageBox();

            //Set Values
            messagebox.SetValues(message, caption, MessageBoxButton.OKCancel, icon, ok_button_text, string.Empty, string.Empty, cancel_button_text);

            //Show MessageBox
            messagebox.ShowDialog();

            //Return Result
            return messagebox.Result;
        }


        // Show Yes / No
        // ========================================================
        // ========================================================
        public static MessageBoxResult ShowYesNo(string message, string caption, string yes_button_text, string no_button_text)
        {
            //Create MessageBox Object
            CustomMessageBox messagebox = new CustomMessageBox();

            //Set Values
            messagebox.SetValues(message, caption, MessageBoxButton.YesNo, MessageBoxImage.None, string.Empty, yes_button_text, no_button_text, string.Empty);

            //Show MessageBox
            messagebox.ShowDialog();

            //Return Result
            return messagebox.Result;
        }

        public static MessageBoxResult ShowYesNo(string message, string caption, string yes_button_text, string no_button_text, MessageBoxImage icon)
        {
            //Create MessageBox Object
            CustomMessageBox messagebox = new CustomMessageBox();

            //Set Values
            messagebox.SetValues(message, caption, MessageBoxButton.YesNo, icon, string.Empty, yes_button_text, no_button_text, string.Empty);

            //Show MessageBox
            messagebox.ShowDialog();

            //Return Result
            return messagebox.Result;
        }


        // Show Yes / No / Cancel
        // ========================================================
        // ========================================================
        public static MessageBoxResult ShowYesNoCancel(string message, string caption, string yes_button_text, string no_button_text, string cancel_button_text)
        {
            //Create MessageBox Object
            CustomMessageBox messagebox = new CustomMessageBox();

            //Set Values
            messagebox.SetValues(message, caption, MessageBoxButton.YesNoCancel, MessageBoxImage.None, string.Empty, yes_button_text, no_button_text, cancel_button_text);

            //Show MessageBox
            messagebox.ShowDialog();

            //Return Result
            return messagebox.Result;
        }

        public static MessageBoxResult ShowYesNoCancel(string message, string caption, string yes_button_text, string no_button_text, string cancel_button_text, MessageBoxImage icon)
        {
            //Create MessageBox Object
            CustomMessageBox messagebox = new CustomMessageBox();

            //Set Values
            messagebox.SetValues(message, caption, MessageBoxButton.YesNoCancel, icon, string.Empty, yes_button_text, no_button_text, cancel_button_text);

            //Show MessageBox
            messagebox.ShowDialog();

            //Return Result
            return messagebox.Result;
        }
        #endregion Custom
        #endregion Show Dialog



        // Event Handlers
        // ========================================================
        // ========================================================
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Get Name
            string name = (sender as Button).Name.Replace("PART_", "").ToLower();

            //Get Result
            Results.TryGetValue(name, out MessageBoxResult result);

            //Set Result
            Result = result;

            //Close Window
            Close();
        }



        // Methods
        // ========================================================
        // ========================================================
        private void DisplayButtons(MessageBoxButton button)
        {
            //Run Switch Statement
            switch (button)
            {
                //Check Message Box Button Type
                case MessageBoxButton.OKCancel:
                    //Show OK and Cancel Buttons and Hide Yes and No Buttons
                    ButtonToggle(new Button[] { PART_OK, PART_Cancel }, new Button[] { PART_Yes, PART_No });

                    //Set Focus to OK Button
                    PART_OK.Focus();
                    break;
                case MessageBoxButton.YesNo:
                    //Show Yes and No Buttons and Hide OK and Cancel Buttons
                    ButtonToggle(new Button[] { PART_Yes, PART_No }, new Button[] { PART_OK, PART_Cancel });

                    //Set Focus to Yes Button
                    PART_Yes.Focus();
                    break;
                case MessageBoxButton.YesNoCancel:
                    //Show Yes, No and Cancel Buttons, and Hide OK Button
                    ButtonToggle(new Button[] { PART_Yes, PART_No, PART_Cancel}, new Button[] { PART_OK });

                    //Set Focus to Yes Button
                    PART_Yes.Focus();
                    break;
                default:
                    //Show OK Button, and Hide Yes, No and Cancel Buttons
                    ButtonToggle(new Button[] { PART_OK }, new Button[] { PART_Yes, PART_No, PART_Cancel });

                    //Set Focus to OK Button
                    PART_OK.Focus();
                    break;
            }
        }

        private void SetIcon(MessageBoxImage icon)
        {
            //Validate Icon
            if (icon != MessageBoxImage.None)
            {
                //Get Icon
                MessageBoxIcons.Icons.TryGetValue(icon, out string value);

                //Set Icon
                MessageBoxIcon = value;
            }
        }



        #region Extensions
        private void SetValues(string message, string caption = "", MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.None, string ok_button_text = "", string yes_button_text = "", string no_button_text = "", string cancel_button_text = "")
        {
            //Set Message
            Message = message;

            //Set Caption
            Caption = caption;

            //Set Button Text
            OK = ok_button_text;
            Yes = yes_button_text;
            No = no_button_text;
            Cancel = cancel_button_text;

            //Set Button Type
            ButtonType = button;

            //Set Icon
            SetIcon(icon);
        }

        private void SetButtonText(Button button, string defaultvalue, string value)
        {
            //Set Button Text
            (button.Content as TextBlock).Text = string.IsNullOrEmpty(value) ? defaultvalue : value;
        }

        private void ButtonToggle(Button[] showbuttons, Button[] hidebuttons)
        {
            //Loop through buttons in buttons array
            foreach (Button button in showbuttons)
            {
                //Show Current Looped Button
                button.Visibility = Visibility.Visible;
            }

            //Loop through buttons in buttons array
            foreach (Button button in hidebuttons)
            {
                //Hide Current Looped Button
                button.Visibility = Visibility.Collapsed;
            }
        }
        #endregion Extensions
    }
}
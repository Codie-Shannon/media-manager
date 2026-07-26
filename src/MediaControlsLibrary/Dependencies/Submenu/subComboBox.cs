using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using MediaControlsLibrary.Dependencies;

namespace MediaControlsLibrary
{
    // Element Template
    // ====================================================
    // ====================================================
    [TemplatePart(Name = ContentWrapper, Type = typeof(ComboBox))]


    // Visual State Templates
    // ====================================================
    // ====================================================
    [TemplateVisualState(GroupName = "subComboBoxStates", Name = ActiveStateName)]
    [TemplateVisualState(GroupName = "subComboBoxStates", Name = InactiveStateName)]


    public class subComboBox : ComboBoxBase
    {
        #region Variables
        // Menu Item States
        // ====================================================
        // ====================================================
        public const string ActiveStateName = "Active";
        public const string InactiveStateName = "Inactive";


        // Content Wrapper Variables
        // ====================================================
        // ====================================================
        public const string ContentWrapper = "PART_Content";
        private Button ContentWrapperButton { get; set; }


        // Variables
        // ====================================================
        // ====================================================
        private subCheckBox SetCheckBox;
        private subRadioButton[] RadioButtons;
        private subCheckBox[] CheckBoxes;


        // ComboBox Event Handler Variable
        // ====================================================
        // ====================================================
        public event EventHandler<SelectionChangedEventArgs> SelectionUpdate;
        #endregion Variables



        // Constructor
        // ====================================================
        // ====================================================
        static subComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(subComboBox), new FrameworkPropertyMetadata(typeof(subComboBox)));
        }



        // Apply Template
        // ====================================================
        // ====================================================
        public override void OnApplyTemplate()
        {
            //Fire When the Element's Template is Applied
            base.OnApplyTemplate();

            //Get Element's Button
            ContentWrapperButton = (Button)GetTemplateChild(ContentWrapper);

            //Set Visual State to Inactive
            VisualStateManager.GoToState(this, InactiveStateName, false);

            //Get ComboBox's Items
            RadioButtons = GetRadioButtons(this.Items);
            CheckBoxes = GetCheckBoxes(this.Items);

            //Set Selected Indexes
            SetIndexes(SelectedRadio, SelectedCheckBox);

            //Set Event Handlers
            ContentWrapperButton.Click += ContentWrapperButton_ButtonClicked;
            this.SelectionChanged += ComboBox_SelectionChanged;
            this.DropDownClosed += ComboBox_DropDownClosed;
        }



        #region Event Handlers
        private void ContentWrapperButton_ButtonClicked(object sender, RoutedEventArgs e)
        {
            //Set Visual State to Active
            SwitchState(ActiveStateName);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Get Value
            GetValue();

            //Change Selection
            ChangeSelection();

            //Invoke Selection Change
            SelectionUpdate?.Invoke(this, e);
        }

        private void ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            //Set Visual State to Inactive
            SwitchState(InactiveStateName);
        }
        #endregion Event Handlers



        #region Methods
        private subRadioButton[] GetRadioButtons(ItemCollection items)
        {
            //Declare radioButtons List
            List<subRadioButton> radioButtons = new List<subRadioButton>();

            //Loop Through Each Item within The Items Collection
            foreach (var item in items)
            {
                //Check if the Current Looped Item is a Radio Button
                if (item.GetType() == typeof(subRadioButton))
                {
                    //Add Radio Button to radioButtons List
                    radioButtons.Add((subRadioButton)item);
                }
            }

            //Return radioButtons List
            return radioButtons.ToArray();
        }

        private subCheckBox[] GetCheckBoxes(ItemCollection items)
        {
            //Declare Set List
            List<subCheckBox> checkboxes = new List<subCheckBox>();

            //Loop Through Each Item within The Items Collection
            foreach (var item in items)
            {
                //Check if the Current Looped Item is a CheckBox
                if (item.GetType() == typeof(subCheckBox))
                {
                    //Add CheckBox to checkBoxes List
                    checkboxes.Add((subCheckBox)item);
                }
            }

            //Return checkBoxes List
            return checkboxes.ToArray();
        }

        public int[] GetIndexes()
        {
            //Variables
            int radio = 0, checkbox = 0;

            //Loop through RadioButtons Array
            for (int i = 0; i < RadioButtons.Length; i++)
            {
                //Check if radio button is checked
                if(RadioButtons[i].IsChecked == true)
                {
                    //Set radio to i
                    radio = i;

                    //Stop For Loop
                    break;
                }
            }

            //Loop through Checkboxes Array
            for (int i = 0; i < CheckBoxes.Length; i++)
            {
                //Check if checkbox is checked
                if (CheckBoxes[i].IsChecked == true)
                {
                    //Set checkbox to i
                    checkbox = i;

                    //Stop For Loop
                    break;
                }
            }

            //Return Indexes
            return new int[] { radio, checkbox };
        }

        public void SetIndexes(int Radio, int CheckBox)
        {
            //Validate Radio Integer
            if(Radio >= 0 && Radio < RadioButtons.Length)
            {
                //Set Selected Index to Set Index
                RadioButtons[Radio].IsChecked = true;

                //Set Order
                this.Order = RadioButtons[Radio].DBName;

                //Set IsReverse Type
                this.IsReverse = RadioButtons[Radio].IsReverse;
            }
            else
            {
                //Set Selected Index to 0
                RadioButtons[0].IsChecked = true;

                //Set Order
                this.Order = RadioButtons[0].DBName;

                //Set IsReverse Order
                this.IsReverse = RadioButtons[0].IsReverse;
            }

            //Validate Checkbox Integer
            if(CheckBox >= 0 && CheckBox < CheckBoxes.Length)
            {
                //Set Selected CheckBox to Set Index
                SetSelectedCheckBox(CheckBox);
            }
            else
            {
                //Set Selected Index to 0
                SetSelectedCheckBox(0);
            }
        }

        private void SetSelectedCheckBox(int index)
        {
            //Check if a checkbox has been set
            if (SetCheckBox != null)
            {
                //Uncheck Set Radio Button
                SetCheckBox.IsChecked = false;
            }

            //Set Selected Index to Set Index
            CheckBoxes[index].IsChecked = true;

            //Set CheckBox
            SetCheckBox = CheckBoxes[index];

            //Set Type
            this.Type = SetCheckBox.DBName;
        }

        private void GetValue()
        {
            //Check if the SelectedItem is of Type RadioButton or CheckBox
            if (this.SelectedItem is subRadioButton)
            {
                //Assign Element's Value to Order
                this.Order = ((subRadioButton)this.SelectedItem).DBName;

                //Assign Element's IsReverse Value to IsReverse
                this.IsReverse = ((subRadioButton)this.SelectedItem).IsReverse;
            }
            else if (this.SelectedItem is subCheckBox)
            {
                //Assign Element's Value to Type
                this.Type = ((subCheckBox)this.SelectedItem).DBName;
            }
        }

        private void ChangeSelection()
        {
            //Check if the Selected Item is of Type RadioButton or CheckBox
            if (this.SelectedItem is subRadioButton)
            {
                //Check Radio Button
                ((subRadioButton)this.SelectedItem).IsChecked = true;
            }
            else if (this.SelectedItem is subCheckBox)
            {
                //Check if the SetCheckBox Variable has been Set
                if (SetCheckBox != null)
                {
                    //Uncheck Checkbox
                    SetCheckBox.IsChecked = false;
                }

                //Set SetCheckBox Variable to Selected Item
                SetCheckBox = (subCheckBox)this.SelectedItem;

                //Check Checkbox
                SetCheckBox.IsChecked = true;
            }
        }

        private void SwitchState(string statename)
        {
            //Change Visual State
            VisualStateManager.GoToState(this, statename, false);
        }
        #endregion Methods
    }
}
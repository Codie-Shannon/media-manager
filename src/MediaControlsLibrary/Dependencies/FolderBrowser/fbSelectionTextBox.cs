using System;
using System.Windows;
using System.Windows.Controls;
using MediaControlsLibrary.Dependencies;

namespace MediaControlsLibrary
{
    public class fbSelectionTextBox : SelectionTextBoxBase
    {
        #region Variables
        // Element Variables
        // ====================================================
        // ====================================================
        private const string str_Content = "PART_Content";
        private TextBox PART_Content { get; set; }


        // TextBox Event Handler Variable
        // ====================================================
        // ====================================================
        public event EventHandler<TextChangedEventArgs> TextUpdated;


        // Other Variables
        // ====================================================
        // ====================================================
        private static bool isInitialCreation;
        #endregion Variables



        // Constructor
        // ========================================================
        // ========================================================
        public fbSelectionTextBox()
        {
            //Check if the element has not been used before
            if (!isInitialCreation)
            {
                //Override Metadata
                DefaultStyleKeyProperty.OverrideMetadata(typeof(fbSelectionTextBox), new FrameworkPropertyMetadata(typeof(fbSelectionTextBox)));

                //Set isInitialCreation to True
                isInitialCreation = true;
            }
        }



        // On Apply Template
        // ====================================================
        // ====================================================
        public override void OnApplyTemplate()
        {
            //Fire When the Element's Template is Applied
            base.OnApplyTemplate();

            //Get TextBox Element
            PART_Content = (TextBox)GetTemplateChild(str_Content);

            //Set Event Handlers
            PART_Content.TextChanged += TextBox_TextChanged;
        }



        // Event Handlers
        // ====================================================
        // ====================================================
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Set DBName to TextBox Text
            this.DBName = PART_Content.Text;

            //Invoke Text Updated
            TextUpdated?.Invoke(this, e);
        }
    }
}
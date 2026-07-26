using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using MediaControlsLibrary.Models;
using MediaControlsLibrary.Dependencies;

namespace MediaControlsLibrary
{
    public class optFolderBrowserDialog : FolderBrowserDialogBase
    {
        #region Variables
        // TextBox Variables
        // ========================================================
        // ========================================================
        private string str_Text = "PART_Text";
        private TextBox PART_Text { get; set; }


        // Button Variables
        // ========================================================
        // ========================================================
        private string str_Button = "PART_Button";
        private Button PART_Button { get; set; }


        // TextBox Event Handler Variable
        // ====================================================
        // ====================================================
        public event EventHandler<TextChangedEventArgs> TextChanged;
        #endregion Variables



        // Constructor
        // ====================================================
        // ====================================================
        static optFolderBrowserDialog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(optFolderBrowserDialog), new FrameworkPropertyMetadata(typeof(optFolderBrowserDialog)));
        }



        // On Apply Template
        // ========================================================
        // ========================================================
        public override void OnApplyTemplate()
        {
            //Fire When the Element's Template is Applied
            base.OnApplyTemplate();

            //Get Elements
            PART_Text = (TextBox)GetTemplateChild(str_Text);
            PART_Button = (Button)GetTemplateChild(str_Button);

            //Set Event Handlers
            PART_Text.TextChanged += OnTextChanged;
            PART_Button.Click += Button_Click;
        }



        // Event Handlers
        // ====================================================
        // ====================================================
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            //Invoke Text Changed
            TextChanged?.Invoke(this, e);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Set Selected Id of Folder Browser to Selected Id of Folder Browser Dialog
            FolderBrowser.EditSelectedId = EditSelectedId;

            //Show Folder Browser Dialog
            Tuple<int, string, Stack<Folder>> folder = FolderBrowser.ShowDialog(InitialFolderID, WindowCaption);

            //Run Folder Selected Method
            FolderSelected(folder);
        }



        #region Methods
        // Folder
        // ====================================================
        // ====================================================
        private void FolderSelected(Tuple<int, string, Stack<Folder>> folder)
        {
            //Variables
            string folderPath = "";

            //Validate Result
            if (folder.Item2 != "Unset")
            {
                //Convert Stack to List
                List<Folder> folders = new List<Folder>(folder.Item3);

                //Reverse List
                folders.Reverse();

                //Set Selected Folder
                Id = folder.Item1;
                DBName = folder.Item2;

                //Check if the Selected Folder is Not Contained within the folders List
                if (!folders.Any(i => i.Id == Id && i.Name == DBName))
                {
                    //Add Selected Folder to folders List
                    folders.Add(new Folder() { Id = folder.Item1, Name = folder.Item2 });
                }

                //Loop through elements in folders List
                for (int i = 0; i < folders.Count; i++)
                {
                    //Check if the current looped folder is not the initial folder
                    if (folders[i].Id != InitialFolderID)
                    {
                        //Create Folder Path
                        folderPath += i != folders.Count - 1 ? $"{folders[i].Name}\\" : $"{folders[i].Name}";
                    }
                }

                //Set Folder Path
                FolderPath = folderPath;
            }
        }


        // Folder
        // ====================================================
        // ====================================================
        public static void AddFolder(int id, int ownerid, string name)
        {
            //Add Folder to Folder Browser
            FolderBrowser.AddFolder(id, ownerid, name);
        }


        // Clear
        // ====================================================
        // ====================================================
        public static void Clear()
        {
            //Clear Folder Browser
            FolderBrowser.Clear();
        }
        #endregion Methods
    }
}
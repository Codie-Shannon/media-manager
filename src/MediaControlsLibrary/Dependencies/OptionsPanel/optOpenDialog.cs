using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System.Windows.Controls;
using MediaControlsLibrary.Dependencies;
using static MediaControlsLibrary.Types.FileTypes;
using static MediaControlsLibrary.Types.FolderPath;
using System.Linq;
using MediaControlsLibrary.Models;
using System.Windows.Input;
using MediaControlsLibrary.Commands;

namespace MediaControlsLibrary
{
    public class optOpenDialog : OpenDialogBase
    {
        #region Variables
        // Content
        // ====================================================
        // ====================================================
        private const string str_Content = "PART_Content";
        private Grid PART_Content;


        // Clear
        // ====================================================
        // ====================================================
        private const string str_Clear = "PART_Clear";
        private Button PART_Clear;


        // Items
        // ====================================================
        // ====================================================
        private const string str_Items = "PART_Items";
        private ItemsControl PART_Items;


        // Button
        // ====================================================
        // ====================================================
        private const string str_BrowseButton = "PART_Button";
        private Button PART_Browse;


        // Event Handlers
        // ====================================================
        // ====================================================
        public event EventHandler<RoutedEventArgs> ClearClick;
        public event EventHandler<RoutedEventArgs> RemoveClick;


        // Other
        // ====================================================
        // ====================================================
        private bool isInitialBrowse = true;
        private int counter = 0;
        #endregion Variables




        // Constructor
        // ====================================================
        // ====================================================
        // ====================================================
        static optOpenDialog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(optOpenDialog), new FrameworkPropertyMetadata(typeof(optOpenDialog)));
        }




        // Apply Template
        // ====================================================
        // ====================================================
        // ====================================================
        public override void OnApplyTemplate()
        {
            //Fire When the Element's Template is Applied
            base.OnApplyTemplate();

            //Get Elements
            PART_Content = (Grid)this.Template.FindName(str_Content, this);
            PART_Items = (ItemsControl)this.Template.FindName(str_Items, this);
            PART_Browse = (Button)this.Template.FindName(str_BrowseButton, this);

            //Set Event Handlers
            PART_Browse.Click += Button_Clicked;

            //Validate Clear Button
            if (IsClear)
            {
                //Get Clear Button Element
                PART_Clear = (Button)this.Template.FindName(str_Clear, this);

                //Set Clear Button Event Handler
                PART_Clear.Click += Clear_Clicked;
            }

            //Validate Multiselect
            if (IsMultiSelection)
            {
                //Get Items Element
                PART_Items = (ItemsControl)this.Template.FindName(str_Items, this);

                //Set Open Dialog Height
                PART_Content.Height = 205;

                //Set Standard Open Dialog Item Properties
                OpenDialogItem.Cover = Cover;
                OpenDialogItem.RemoveHeader = RemoveHeader;

                //Set Items Source
                PART_Items.ItemsSource = Contents;

                //Set Remove Click Command
                PART_Content.CommandBindings.Add(new CommandBinding(OpenDialogCommands.RemoveClick, Remove_Click));
            }
        }




        #region Event Handlers
        // Clear Button
        // ====================================================
        // ====================================================
        private void Clear_Clicked(object sender, RoutedEventArgs e)
        {
            //Check if the Clear Event is Invalid
            if (string.IsNullOrEmpty($"{Content}".Trim()))
            {
                //Set Routed Event Arguments as Handled
                e.Handled = true;

                //Return Method
                return;
            }

            //Clear Content
            Content = string.Empty;

            //Invoke Clear Click
            ClearClick?.Invoke(this, e);
        }



        // Browse Button
        // ====================================================
        // ====================================================
        private void Button_Clicked(object sender, RoutedEventArgs e)
        {
            //Check if the OpenFileDialog Type is File or Folder
            if (Type == FileType.File)
            {
                //Open File Dialog Box
                OpenFileDialog(e);
            }
            else if (Type == FileType.Folder)
            {
                //Open Folder Dialog Box
                OpenFolderDialog();
            }
        }



        // Remove Click
        // ====================================================
        // ====================================================
        private void Remove_Click(object sender, ExecutedRoutedEventArgs e)
        {
            //Convert Original Source (Item to be Removed) to Button
            Button item = (Button)e.OriginalSource;

            //Try Parse Uid to Integer
            int.TryParse(item.Uid, out int id);

            //Validate Item
            if (Contents.Any(i => i.Id == id))
            {
                //Remove Item from Contents Observable Collection
                Contents.Remove(Contents.Single(i => i.Id == id));
            }

            //Invoke Remove Click
            RemoveClick?.Invoke(this, e);
        }
        #endregion Event Handlers




        #region Methods
        // Open Folder Dialog
        // ====================================================
        // ====================================================
        public void OpenFolderDialog()
        {
            //Create New VistaFolderBrowserDialog Object
            VistaFolderBrowserDialog openFolderDialog = new VistaFolderBrowserDialog();

            //Check if isInitialBrowse is Set to True
            if (isInitialBrowse == true)
            {
                //Set Initial Directory
                openFolderDialog.SelectedPath = GetDirectory();
            }

            //Show OpenFolderDialog Box
            Nullable<bool> result = openFolderDialog.ShowDialog();

            //Check if the OpenFolderDialog Box has got a Folder
            if (result == true)
            {
                //Set isInitialBrowse to False
                isInitialBrowse = false;

                //Set Content to Selected Folder Path
                this.Content = openFolderDialog.SelectedPath;
            }
        }



        // Open File Dialog
        // ====================================================
        // ====================================================
        public void OpenFileDialog(RoutedEventArgs e)
        {
            //Create New OpenFileDialog Object
            OpenFileDialog openFileDialog = new OpenFileDialog();

            //Set Filter
            openFileDialog.Filter = SelectionName + "|" + GetFileTypes(Selectables.Split(new string[] { ", " }, StringSplitOptions.None));

            //Set Multiselect
            openFileDialog.Multiselect = IsMultiSelection;

            //Check if isInitialBrowse is Set to True
            if (isInitialBrowse == true)
            {
                //Set Initial Directory
                openFileDialog.InitialDirectory = GetDirectory();
            }

            //Show OpenFileDialog Box
            Nullable<bool> result = openFileDialog.ShowDialog();

            //Check if the result was invalid
            if(result == false)
            {
                //Set Event to Handled
                e.Handled = true;

                //Return Event
                return;
            }

            //Check if the OpenFileDialog Box has got a File
            if (result == true)
            {
                //Set isInitialBrowse to False
                isInitialBrowse = false;

                //Validate Selection Type
                if (IsMultiSelection)
                {
                    //Get Multiselect Items
                    string[] files = openFileDialog.FileNames;

                    //Validate Files
                    if(files.Length > 0)
                    {
                        //Set Browse Path to Directory
                        this.Content = Path.GetDirectoryName(files[0]);
                    }

                    //Loop through files Array
                    foreach (var file in files.ToList().OrderBy(i => i))
                    {
                        //Add Current Looped File to Contents Observable Collection
                        this.Contents.Add(new OpenDialogItem() { Id = counter, FilePath = file, Name = Path.GetFileNameWithoutExtension(file) });

                        //Increment Selection Counter
                        counter++;
                    }
                }
                else
                {
                    //Set Content to Selected File Path
                    this.Content = openFileDialog.FileName;
                }
            }
        }
        #endregion Methods




        #region Extensions
        // Get File Types
        // ====================================================
        // ====================================================
        public string GetFileTypes(string[] fileTypes)
        {
            //Declare str_fileTypes String
            string str_fileTypes = "";

            //Format File Types
            for (int i = 0; i < fileTypes.Length; i++)
            {
                str_fileTypes += "*." + fileTypes[i];

                if (i != fileTypes.Length - 1)
                {
                    str_fileTypes += ";";
                }
            }

            //Return Formatted File Types
            return str_fileTypes;
        }



        // Get Directory
        // ====================================================
        // ====================================================
        public string GetDirectory()
        {
            //Validate Initial Directory
            if (!string.IsNullOrEmpty(InitialDirectory) && Directory.Exists(InitialDirectory) && InitialDirectory != Directory.GetDirectoryRoot(InitialDirectory))
            {
                //Return Initial Directory
                return InitialDirectory.EndsWith(@"\") ? InitialDirectory : $"{InitialDirectory}\\";
            }
            else
            {
                //Get Root Directory
                FolderPaths.TryGetValue(RootDirectory, out string path);

                //Return Root Directory
                return path;
            }
        }
        #endregion Extensions
    }
}
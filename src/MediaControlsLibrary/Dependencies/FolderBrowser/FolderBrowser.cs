using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Collections.Generic;
using MediaControlsLibrary.Models;
using MediaControlsLibrary.Commands;

namespace MediaControlsLibrary
{
    public class FolderBrowser : Window
    {
        #region Variables
        // Caption Variables
        // ========================================================
        // ========================================================
        private string Caption { get { return Title; } set { Title = value; } }


        // Navigation Bar Variables
        // ========================================================
        // ========================================================
        private string str_Navigation = "PART_Navigation";
        private NavigationBar PART_Navigation { get; set; }


        // Item Controls Variables
        // ========================================================
        // ========================================================
        private string str_Content = "PART_Content";
        private ItemsControl PART_Content { get; set; }


        // TextBox Variables
        // ========================================================
        // ========================================================
        private string str_TextBox = "PART_TextBox";
        private fbSelectionTextBox PART_TextBox { get; set; }


        // Select Button Variables
        // ========================================================
        // ========================================================
        private string str_Select = "PART_Select";
        private Button PART_Select { get; set; }


        // Cancel Button Variables
        // ========================================================
        // ========================================================
        private string str_Cancel = "PART_Cancel";
        private Button PART_Cancel { get; set; }


        // Folders Variables
        // ========================================================
        // ========================================================
        private static List<Folder> Folders = new List<Folder>();
        private static List<Folder> FoldersReserve = new List<Folder>();


        // Selection Variables
        // ========================================================
        // ========================================================
        private static CoverFolder selectedElement;
        private static Folder selectedFolder;
        private static int ActiveFolder;


        // Close Variables
        // ========================================================
        // ========================================================
        private DispatcherTimer CloseDispatcher = null;
        private bool isClosing = false;


        // Other Variables
        // ========================================================
        // ========================================================
        public static int EditSelectedId;
        private static bool isInitialCreation;
        private string NoSelectionError = "No folder is currently selected. Please select a folder to continue.";
        #endregion Variables



        // Fields
        // ========================================================
        // ========================================================
        public static readonly DependencyProperty MaxLengthProperty = DependencyProperty.Register(nameof(MaxLength), typeof(int), typeof(FolderBrowser), new PropertyMetadata(20));
        public static DependencyProperty selectedIdProperty = DependencyProperty.Register(nameof(selectedId), typeof(int), typeof(FolderBrowser), new PropertyMetadata(-1));



        // Properties
        // ========================================================
        // ========================================================
        public int MaxLength
        {
            get => (int)GetValue(MaxLengthProperty);
            set => SetValue(MaxLengthProperty, value);
        }

        public int selectedId
        {
            get => (int)GetValue(selectedIdProperty);
            set => SetValue(selectedIdProperty, value);
        }



        // Constructor
        // ========================================================
        // ========================================================
        public FolderBrowser()
        {
            //Check if the window has not been opened before
            if (!isInitialCreation)
            {
                //Override Metadata
                DefaultStyleKeyProperty.OverrideMetadata(typeof(FolderBrowser), new FrameworkPropertyMetadata(typeof(FolderBrowser)));

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



        // On Apply Template
        // ========================================================
        // ========================================================
        public override void OnApplyTemplate()
        {
            //Fire When the Element's Template is Applied
            base.OnApplyTemplate();

            //Set Startup Location
            SetStartupLocation();

            //Get Elements
            PART_Navigation = (NavigationBar)GetTemplateChild(str_Navigation);
            PART_Content = (ItemsControl)GetTemplateChild(str_Content);
            PART_TextBox = (fbSelectionTextBox)GetTemplateChild(str_TextBox);
            PART_Select = (Button)GetTemplateChild(str_Select);
            PART_Cancel = (Button)GetTemplateChild(str_Cancel);

            //Set Event Handlers
            Loaded += OnLoaded;
            Closing += WindowClosing;
            PART_Navigation.FolderClick += NavigationBar_Click;
            PART_Navigation.Back += NavigationBar_Click;
            PART_Navigation.Forward += NavigationBar_Click;
            PART_Content.MouseUp += Content_MouseUp;
            PART_TextBox.TextUpdated += TextBox_TextUpdated;
            PART_Select.Click += Select_Click;
            PART_Cancel.Click += Cancel_Click;
            
            //Set Folder Click Command
            PART_Content.CommandBindings.Add(new CommandBinding(FolderBrowserCommands.FolderClick, Folder_Click));

            //Set Content's Items Source
            SetItemsSource();
        }



        #region Event Handlers
        // Window
        // ========================================================
        // ========================================================
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            //Setup Navigation Bar
            SetupNavigation();
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            //Check if isClosing is Set to False
            if (isClosing == false)
            {
                //Cancel Event
                e.Cancel = true;

                //Set isClosing to True
                isClosing = true;

                //Setup CloseDispatcher for Close
                CloseDispatcher_Setup();
            }
            else
            {
                //Do not retain a visual owned by a window that is being destroyed.
                selectedElement = null;
            }
        }


        // Navigation
        // ========================================================
        // ========================================================
        private void NavigationBar_Click(object sender, RoutedEventArgs e)
        {
            //Navigate to Folder
            NavigateToFolder();
        }


        // Items Control
        // ========================================================
        // ========================================================
        private void Content_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //Deselect Folder
            DeselectFolder();
        }


        // Folder
        // ========================================================
        // ========================================================
        private void Folder_Click(object sender, ExecutedRoutedEventArgs e)
        {
            //Get Folder. Keyboard and UI Automation invocations can raise the
            //command from the CoverFolder itself instead of its template button.
            CoverFolder folder = e.OriginalSource as CoverFolder
                ?? (e.OriginalSource as FrameworkElement)?.TemplatedParent as CoverFolder;

            if (folder == null)
            {
                return;
            }

            //Select Folder
            SelectFolder(folder);
        }

        // TextBox
        // ========================================================
        // ========================================================
        private void TextBox_TextUpdated(object sender, TextChangedEventArgs e)
        {
            //Search Folder
            SearchFolder();
        }


        // Buttons
        // ========================================================
        // ========================================================
        private void Select_Click(object sender, RoutedEventArgs e)
        {
            //Confirm the Folder Selection
            ConfirmFolderSelection();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            //Close Window
            Close();
        }
        #endregion Event Handlers



        #region Methods
        // Show Dialog
        // ========================================================
        // ========================================================
        public static KeyValuePair<int, string> Show(int defaultfolder, string caption)
        {
            ResetSelection();

            //Create FolderBrowser Object
            FolderBrowser folderBrowser = new FolderBrowser();

            //Set Caption
            folderBrowser.Caption = string.IsNullOrEmpty(caption) ? "Select Folder" : caption;

            //Set Active Folder to Default Folder
            ActiveFolder = defaultfolder;

            //Set Folders List
            Folders = FoldersReserve.Where(i => i.OwnerId == ActiveFolder).ToList();

            //Show Folder Browser
            folderBrowser.ShowDialog();

            //Return Result
            return selectedFolder == null ? new KeyValuePair<int, string>(0, "Unset") : new KeyValuePair<int, string>(selectedFolder.Id, selectedFolder.Name);
        }

        public static Tuple<int, string, Stack<Folder>> ShowDialog(int defaultfolder, string caption)
        {
            ResetSelection();

            //Create FolderBrowser Object
            FolderBrowser folderBrowser = new FolderBrowser();

            //Set Caption
            folderBrowser.Caption = string.IsNullOrEmpty(caption) ? "Select Folder" : caption;

            //Set Active Folder to Default Folder
            ActiveFolder = defaultfolder;

            //Set Folders List
            Folders = FoldersReserve.Where(i => i.OwnerId == ActiveFolder).ToList();

            //Show Folder Browser
            folderBrowser.ShowDialog();

            //Return Result
            return selectedFolder == null ? Tuple.Create(0, "Unset", new Stack<Folder>()) : Tuple.Create(selectedFolder.Id, selectedFolder.Name, NavigationBar.selectedStack);
        }


        // Navigation
        // ========================================================
        // ========================================================
        private void SetupNavigation()
        {
            //Loop through elements in FoldersReserve
            foreach (Folder folder in FoldersReserve)
            {
                //Add Folder to Navigation Bar
                PART_Navigation.Add(folder.Id, folder.OwnerId, folder.Name, folder.Type);
            }

            //Load Default Folder
            PART_Navigation.Load(ActiveFolder);
        }

        private void NavigateToFolder()
        {
            //Set Active Folder to Navigation's Bar Current Loaded Folder's ID
            ActiveFolder = PART_Navigation.selectedId;

            //Unset selectedId
            selectedId = -1;

            //Set Folders List
            Folders = FoldersReserve.Where(i => i.OwnerId == PART_Navigation.selectedId && i.Id != EditSelectedId).ToList();

            //Validate Folder ID
            if (PART_Navigation.selectedId == -1)
            {
                //Deselect Folder
                DeselectFolder();
            }
            else
            {
                //Set selectedFolder to the Navigation's Bar Current Loaded Folder
                selectedFolder = FoldersReserve.Single(i => i.Id == PART_Navigation.selectedId);

                //Set TextBox Text to the Navigation's Bar Current Loaded Folder's Name
                PART_TextBox.DBName = FoldersReserve.Single(i => i.Id == PART_Navigation.selectedId).Name;
            }

            //Set Content's Items Source
            SetItemsSource();
        }


        // Folder
        // ========================================================
        // ========================================================
        public static void AddFolder(int id, int ownerid, string name)
        {
            //Check if the Folders List Already Contains the Specified Folder
            FoldersReserve.RemoveAll(i => i.Id == id && i.OwnerId == ownerid);

            //Add Folder to Folders List
            FoldersReserve.Add(new Folder() { Id = id, OwnerId = ownerid, Name = name });
        }

        private void SelectFolder(CoverFolder element, bool isSearch = false)
        {
            //Get Selected Folder ID
            int.TryParse(element.Tag.ToString(), out int id);

            //Set selectedFolder to the Loaded Folder within the Navigation Bar
            Folder folder = Folders.Single(i => i.Id == id);

            //Check if the folder was clicked twice
            if (folder == selectedFolder && isSearch == false)
            {
                //Load the Folder within the Navigation Bar
                PART_Navigation.Load(id);

                //Set the Active Folder to the Current Loaded Folder within the Navigation Bar
                ActiveFolder = id;

                //Set Folders List
                Folders = FoldersReserve.Where(i => i.OwnerId == id && i.Id != EditSelectedId).ToList();

                //Set Content's Items Source
                SetItemsSource();
            }
            else
            {
                //Check if a Element is Currently Selected
                if (selectedElement != null)
                {
                    //Deselect Element
                    selectedElement.IsSelected = false;
                }

                //Set selectedFolder to folder
                selectedFolder = folder;

                //Set selectedId to selectedFolder.Id
                selectedId = selectedFolder.Id;

                //Set selectedElement to Sender
                selectedElement = element;

                //Select Element
                selectedElement.IsSelected = true;

                //Check if isSearch is Set to False
                if (!isSearch)
                {
                    //Set TextBox Text
                    PART_TextBox.DBName = selectedFolder.Name;
                }
            }
        }

        private void DeselectFolder()
        {
            //Check if a folder is currently selected
            if (selectedElement != null && selectedFolder != null)
            {
                //Deselect Element
                selectedElement.IsSelected = false;
                PART_TextBox.DBName = string.Empty;

                //Set Selected Variables to Null
                selectedElement = null;
                selectedFolder = null;
                selectedId = -1;
            }
        }

        private async void SearchFolder()
        {
            //Get PART_TextBox's TextBox
            TextBox part_content = (TextBox)PART_TextBox.Template.FindName("PART_Content", PART_TextBox);

            //Check if PART_TextBox's TextBox is Currently in Focus (i.e. the User is Currently Searching for an Item)
            if (part_content.IsFocused)
            {
                //Initialize Variables
                bool isselected = false;
                string search = PART_TextBox.DBName.ToLower();

                //Delay Task
                await Task.Delay(100);

                //Check if the FoldersReserve Contains Any Elements with the OwnerId of ActiveFolder and the Searched Text Contained within the Name
                if (!string.IsNullOrEmpty(PART_TextBox.DBName) && FoldersReserve.Any(i => i.OwnerId == ActiveFolder && i.Name.ToLower().Contains(search)))
                {
                    //Set Folders List
                    Folders = FoldersReserve.Where(i => i.OwnerId == ActiveFolder && i.Name.ToLower().Contains(PART_TextBox.DBName.ToLower()) && i.Id != EditSelectedId).ToList();

                    //Set Content's Items Source
                    SetItemsSource();
                }
                else
                {
                    //Set Folders List
                    Folders = FoldersReserve.Where(i => i.OwnerId == ActiveFolder && i.Id != EditSelectedId).ToList();

                    //Set Content's Items Source
                    SetItemsSource();
                }

                //Check if the Folders List Contains a Match for the Searched Text and if the selectedFolder is not Set to the Searched Folder or if the Folders List Count is 1 and and the Folders Element is not Selected
                if (!string.IsNullOrEmpty(PART_TextBox.DBName) && (Folders.Any(i => i.Name.ToLower() == search) && Folders.Single(i => i.Name.ToLower() == search) != selectedFolder || Folders.Count() == 1 && Folders.First() != selectedFolder))
                {
                    //Set selectedFolder to First or Searched Folder (Dependent on Folders Count) 
                    selectedFolder = Folders.Count() == 1 ? Folders.First() : Folders.Single(i => i.Name.ToLower() == PART_TextBox.DBName.ToLower());

                    //Set isselected to True
                    isselected = true;
                }

                //Delay Task
                await Task.Delay(50);

                //Check if isselected is Set to True and if selectedFolder has been Set
                if (isselected && selectedFolder != null)
                {
                    //Get and Loop through Folder UI Elements
                    foreach (CoverFolder folder in FindVisualChildren<CoverFolder>(PART_Content))
                    {
                        //Check if the Current Looped folder.Tag is Set to the Current selectedFolder.Id
                        if (folder.Tag.ToString() == selectedFolder.Id.ToString())
                        {
                            //Select Folder
                            SelectFolder(folder, isselected);

                            //Delay Task
                            await Task.Delay(10);

                            //Set Fcous to PART_TextBox's TextBox
                            part_content.Focus();
                            Keyboard.Focus(part_content);

                            //Set Cursor to End of PART_TextBox's TextBox
                            part_content.CaretIndex = part_content.Text.Length;
                        }
                    }
                }
            }
        }


        // Clear
        // ========================================================
        // ========================================================
        public static void Clear()
        {
            //Reset Folders
            Folders = new List<Folder>();
            FoldersReserve = new List<Folder>();

            //Reset Selected Variables
            ResetSelection();

            //Reset Active Folder
            ActiveFolder = 0;
        }

        private static void ResetSelection()
        {
            selectedElement = null;
            selectedFolder = null;
        }


        // Close
        // ========================================================
        // ========================================================
        private void ConfirmFolderSelection()
        {
            //Validate Selected Folder
            if (selectedFolder != null)
            {
                //Close Window
                Close();
            }
            else
            {
                //Show Error Message
                CustomMessageBox.ShowDialog(NoSelectionError, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseDispatcher_Setup()
        {
            //Setup CloseDispatcher for Close
            CloseDispatcher = new DispatcherTimer();
            CloseDispatcher.Tick += new EventHandler(Close_Tick);
            CloseDispatcher.Interval = new TimeSpan(0, 0, 0, 0, 10);
            CloseDispatcher.Start();
        }

        private void Close_Tick(object sender, EventArgs e)
        {
            CloseDispatcher.Stop();
            CloseDispatcher.Tick -= Close_Tick;
            CloseDispatcher = null;

            //Close Window
            Close();
        }
        #endregion Methods



        #region Extensions
        // Find Visual Children
        // ========================================================
        // ========================================================
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            //Check if the Dependency Object has been Set
            if (depObj != null)
            {
                //Loop Through Visual Tree Helper Children of Dependency Object
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    //Get Visual Tree Helper Child at Position i of the Dependency Object
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);

                    //Check if the Child Dependency Object has been Set
                    if (child != null && child is T)
                    {
                        //Cast Child Dependency Object to Type T and Return It
                        yield return (T)child;
                    }

                    //Loop Through Visual Tree Children of Child Dependency Object
                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        //Return Child of Child
                        yield return childOfChild;
                    }
                }
            }
        }


        // Set Items Source
        // ========================================================
        // ========================================================
        private void SetItemsSource()
        {
            //Unset Items Source
            PART_Content.ItemsSource = null;

            //Set Items Source
            PART_Content.ItemsSource = Folders;
        }
        #endregion Extensions
    }
}

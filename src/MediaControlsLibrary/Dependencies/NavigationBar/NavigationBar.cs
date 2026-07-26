using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using MediaControlsLibrary.Models;
using MediaControlsLibrary.Extensions;
using static MediaControlsLibrary.NavigationBarStackTypes;
using MediaControlsLibrary.Types;

namespace MediaControlsLibrary
{
    public class NavigationBar : ItemsControl
    {
        #region Variables
        // Back Button Element
        // ====================================================
        // ====================================================
        private const string str_Back = "PART_Back";
        private Button btnBack { get; set; }
        public event EventHandler<RoutedEventArgs> Back;


        // Forward Button Element
        // ====================================================
        // ====================================================
        private const string str_Forward = "PART_Forward";
        private Button btnForward { get; set; }
        public event EventHandler<RoutedEventArgs> Forward;


        // Item Container Border Element
        // ====================================================
        // ====================================================
        private const string str_Content = "PART_Content";
        private Border bContent { get; set; }


        // Behavioral
        // ====================================================
        // ====================================================
        private new Stack<Stack<Folder>> Loaded = new Stack<Stack<Folder>>(), Unloaded = new Stack<Stack<Folder>>();
        private List<Folder> Folders = new List<Folder>();
        public event EventHandler<RoutedEventArgs> FolderClick;
        private double ElementWidth = 0;
        public static Stack<Folder> selectedStack;
        public int selectedId;
        public string selectedFolderType;
        #endregion Variables



        // Constructor
        // ====================================================
        // ====================================================
        static NavigationBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigationBar), new FrameworkPropertyMetadata(typeof(NavigationBar)));
        }



        // Apply Template
        // ====================================================
        // ====================================================
        public override void OnApplyTemplate()
        {
            //Fire When the Element's Template is Applied
            base.OnApplyTemplate();

            //Get Elements
            btnBack = (Button)GetTemplateChild(str_Back);
            btnForward = (Button)GetTemplateChild(str_Forward);
            bContent = (Border)GetTemplateChild(str_Content);

            //Set Event Handlers
            bContent.SizeChanged += Bar_SizeChanged;
            btnBack.Click += Back_Click;
            btnForward.Click += Forward_Click;
        }



        #region Event Handlers
        // Bar Size Changed
        // ====================================================
        // ====================================================
        private void Bar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //Refresh Folders
            Display();
        }


        // Folder Clicked
        // ====================================================
        // ====================================================
        private void Folder_Clicked(object sender, RoutedEventArgs e)
        {
            //Run Folder Clicked Method
            FolderClicked(sender as NavigationBarItem);

            //Invoke FolderClick Event Handler
            FolderClick?.Invoke(sender, e);
        }


        // Back
        // ====================================================
        // ====================================================
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            //Run Go Back Method
            GoBack();

            //Invoke Back Event Handler
            Back?.Invoke(this, e);
        }


        // Forward
        // ====================================================
        // ====================================================
        private void Forward_Click(object sender, RoutedEventArgs e)
        {
            //Run Go Forward Method
            GoForward();

            //Invoke Forward Event Handler
            Forward?.Invoke(this, e);
        }
        #endregion Event Handlers



        #region Methods
        // Add
        // ====================================================
        // ====================================================
        public void Add(int id, int ownerid, string name, string foldertype)
        {
            //Check if the Folders List Already Contains the Specified Folder
            if (Folders.Any(i => i.Id == id && i.FolderType == foldertype))
            {
                //Remove Folder from Folders List
                Folders.RemoveAt(Folders.IndexOf(Folders.Single(i => i.Id == id && i.FolderType == foldertype)));
            }

            //Add Folder to Folders List
            Folders.Add(new Folder() { Id = id, OwnerId = ownerid, Name = name, FolderType = foldertype });
        }


        // Load
        // ====================================================
        // ====================================================
        public void Load(int id, string foldertype = "")
        {
            //Validate that Folders Contains Elements
            if (Folders.Count > 0)
            {
                //Variables
                Stack<Folder> folders;

                //Check if any elements exist within the Loaded stack
                if (Loaded.Count > 0)
                {
                    //Copy Last Element Added to Loaded Stack
                    folders = Loaded.Copy();
                }
                else
                {
                    //Create New Stack
                    folders = new Stack<Folder>();
                }

                //Add Folder to Element
                folders.Push(Folders.Single(i => i.Id == id && (string.IsNullOrEmpty(foldertype) || (!string.IsNullOrEmpty(foldertype) && i.FolderType == foldertype))));

                //Add folders Stack to Loaded Stack
                Loaded.Push(folders);

                //Set selectedStack
                selectedStack = folders;

                //Set selectedId and selectedFolderType
                selectedId = folders.Peek().Id;
                selectedFolderType = folders.Peek().FolderType;

                //Clear Unloaded Stack
                Unloaded.Clear();

                //Display Folders
                Display();
            }
        }


        // Display
        // ====================================================
        // ====================================================
        private void Display()
        {
            //Check if the Loaded Stack Contains any Elements
            if (Loaded.Count > 0)
            {
                //Remove All Elements From Navigation Bar
                Items.Clear();

                //Get Last Element in Loaded Stack and Convert it to a List
                List<Folder> folders = Loaded.Peek().ToList();

                //Reset Element Width
                ElementWidth = 0;

                //Loop through folders list
                for (int i = 0; i < folders.Count; i++)
                {
                    //Variables
                    double elementwidth = 0;

                    //Create Elements
                    NavigationBarItem item = new NavigationBarItem() { Id = folders[i].Id, OwnerId = folders[i].OwnerId, DBName = folders[i].Name, FolderType = folders[i].FolderType };
                    NavigationBarItemSeparator separator = new NavigationBarItemSeparator();

                    //Add Click Event Handler to item
                    item.Click += Folder_Clicked;

                    //Add Item to Navigation Bar
                    Items.Insert(0, item);

                    //Check if the Current Looped Folder is not the Last Folder within the folders List
                    if (i != folders.Count - 1)
                    {
                        //Add Separator to Navigation Bar
                        Items.Insert(0, separator);
                    }

                    //Measure Elements
                    item.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    separator.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

                    //Get Elements Shared Width
                    elementwidth += item.DesiredSize.Width;
                    elementwidth += separator.DesiredSize.Width;

                    //Add Elements Shared Width to ElementWidth Variable
                    ElementWidth += elementwidth;

                    //Check if the Elements Shared Width is Invalid
                    if (ElementWidth > bContent.ActualWidth && i > 0)
                    {
                        //Check if it is the Last Folder in the List
                        if (i == Loaded.Count - 1)
                        {
                            //Remove Last Two Elements from Items
                            Items.RemoveAt(0);
                            Items.RemoveAt(0);
                        }
                        else
                        {
                            //Remove Last Three Elements from Items
                            Items.RemoveAt(0);
                            Items.RemoveAt(0);
                            Items.RemoveAt(0);
                        }

                        //Stop For Loop
                        break;
                    }
                }
            }

            //Toggle Buttons
            ToggleButtons();
        }


        // Folder Clicked
        // ====================================================
        // ====================================================
        private void FolderClicked(NavigationBarItem item)
        {
            //Validate Click
            if (item.Id != selectedId || item.FolderType != selectedFolderType)
            {
                //Copy Last Element Added to Loaded Stack
                Stack<Folder> folders = Loaded.Copy();

                //Get Count of folders Stack
                int count = folders.Count;

                //Loop through elements in folders stack
                for (int i = 0; i < count; i++)
                {
                    //Remove Last Element Added to folders Stack
                    Folder folder = folders.Pop();

                    //Check if the Current Looped Element Id and FolderType Match the values of item
                    if (folder.Id.ToString() == item.Id.ToString() && folder.FolderType == item.FolderType)
                    {
                        //Add Last Element Added to folders Stack Back to Folders Stack
                        folders.Push(folder);

                        //Stop For Loop
                        break;
                    }
                }

                //Add folders Stack to Loaded Stack
                Loaded.Push(folders);

                //Set selectedStack
                selectedStack = folders;

                //Set selectedId and selectedFolderType
                selectedId = folders.Peek().Id;
                selectedFolderType = folders.Peek().FolderType;

                //Clear Unloaded Stack
                Unloaded.Clear();

                //Display Folders
                Display();
            }
        }


        // Get
        // ====================================================
        // ====================================================
        public string GetFolderPath()
        {
            //Variables
            string folderpath = "";

            //Get Elements in Current Loaded Navigation Stack
            List<Folder> folders = Loaded.Peek().ToList();

            //Reverse List
            folders.Reverse();

            //Loop through folders List
            for (int i = 0; i < folders.Count; i++)
            {
                //Check if the current looped element is not the last element within the list
                if (i < folders.Count - 1)
                {
                    //Add folder to folder path
                    folderpath += $"{folders[i].Name}\\";
                }
                else
                {
                    //Add folder to folder path
                    folderpath += folders[i].Name;
                }
            }

            //Return folderpath
            return folderpath;
        }

        public FolderType GetFolderType()
        {
            //Get Folder Type
            return selectedFolderType == nameof(FolderType.Folders) ? FolderType.Folders : (selectedFolderType == nameof(FolderType.TVShowFolders) ? FolderType.TVShowFolders : FolderType.SeasonFolders);
        }


        // Remove
        // ====================================================
        // ====================================================
        public void Remove(int id, int ownerid, string name, string type)
        {
            //Remove Element From Folders
            Folders.Remove(new Folder() { Id = id, OwnerId = ownerid, Name = name, FolderType = type });

            //Clear Navigation
            ClearNavigation();
        }


        // Clear
        // ====================================================
        // ====================================================
        public void ClearAll() { Folders.Clear(); ClearNavigation(); }

        public void ClearNavigation()
        {
            //Check if Loaded Contains any Elements
            if (Loaded.Count > 0)
            {
                //Get First Element Added to Stack
                Stack<Folder> folders = Loaded.ElementAt(0);

                //Clear Loaded Stack
                Loaded.Clear();

                //Add Element Back to Loaded Stack
                Loaded.Push(folders);
            }

            //Clear Unloaded Stack
            Unloaded.Clear();

            //Toggle Buttons
            ToggleButtons();
        }


        // Go Back and Forward Methods
        // ====================================================
        // ====================================================
        private void GoBack()
        {
            //Remove Last Added Element From Loaded Stack
            Stack<Folder> folder = Loaded.Pop();

            //Add Removed Element to Unloaded Stack
            Unloaded.Push(folder);

            //Set selectedStack
            selectedStack = Loaded.Peek();

            //Set selectedId and selectedFolderType
            selectedId = Loaded.Peek().Peek().Id;
            selectedFolderType = Loaded.Peek().Peek().FolderType;

            //Display Folders
            Display();
        }

        private void GoForward()
        {
            //Remove Last Added Element From Unloaded Stack
            Stack<Folder> folder = Unloaded.Pop();

            //Add Removed Element to Loaded Stack
            Loaded.Push(folder);

            //Set selectedStack
            selectedStack = folder;

            //Set selectedId and selectedFolderType
            selectedId = folder.Peek().Id;
            selectedFolderType = folder.Peek().FolderType;

            //Display Folders
            Display();
        }


        // Stack
        // ====================================================
        // ====================================================
        public Stack<Stack<Tuple<int, int, string, string>>> GetStack(NavigationBarStackType type)
        {
            //Variables
            Stack<Stack<Tuple<int, int, string, string>>> stack;

            //Validate Type
            if (type == NavigationBarStackType.Unloaded)
            {
                //Get Stack from Original Source of Unloaded
                stack = GetStackOriginalSource(Unloaded);
            }
            else
            {
                //Get Stack from Original Source of Loaded
                stack = GetStackOriginalSource(Loaded);
            }

            //Return Stack
            return stack;
        }

        public void SetStack(NavigationBarStackType type, Stack<Stack<Tuple<int, int, string, string>>> values)
        {
            //Variables
            Stack<Stack<Folder>> stack;

            //Validate Type
            if (type == NavigationBarStackType.Unloaded)
            {
                //Get Unloaded Stack Compilation
                stack = GetStackCompilation(values);

                //Set Unloaded Stack
                Unloaded = stack;
            }
            else if (type == NavigationBarStackType.Loaded)
            {
                //Get Loaded Stack Compilation
                stack = GetStackCompilation(values);

                //Set Loaded Stack
                Loaded = stack;

                //Display
                Display();
            }
        }
        #endregion Methods



        #region Extensions
        // Toggle Buttons
        // ====================================================
        // ====================================================
        private void ToggleButtons()
        {
            //Toggle Back Button
            btnBack.IsEnabled = Loaded.Count == 1 ? false : true;

            //Toggle Forward Button
            btnForward.IsEnabled = Unloaded.Count == 0 ? false : true;
        }


        // Stack
        // ====================================================
        // ====================================================
        public Stack<Stack<Tuple<int, int, string, string>>> GetStackOriginalSource(Stack<Stack<Folder>> folders)
        {
            //Variables
            Stack<Stack<Tuple<int, int, string, string>>> newstacks = new Stack<Stack<Tuple<int, int, string, string>>>();

            //Convert Loaded Stack to List
            List<Stack<Folder>> stacks = folders.ToList();

            //Loop through elements in stacks
            for (int i = 0; i < stacks.Count; i++)
            {
                //Create New Stack
                Stack<Tuple<int, int, string, string>> newstack = new Stack<Tuple<int, int, string, string>>();

                //Get Stack at Index i from stacks and convert it to list
                List<Folder> stack = stacks[i].ToList();

                //Loop through elements in stack
                for (int j = 0; j < stack.Count; j++)
                {
                    //Add Current Looped Element to newstack
                    newstack.Push(Tuple.Create(stack[j].Id, stack[j].OwnerId, stack[j].Name, stack[j].FolderType));
                }

                //Add newstack to newstacks
                newstacks.Push(newstack);
            }

            //Return newstacks
            return newstacks;
        }

        public Stack<Stack<Folder>> GetStackCompilation(Stack<Stack<Tuple<int, int, string, string>>> values)
        {
            //Variables
            Stack<Stack<Folder>> newstacks = new Stack<Stack<Folder>>();

            //Convert Parsed Stack to List
            List<Stack<Tuple<int, int, string, string>>> stacks = values.ToList();

            //Loop through elements in stacks
            for (int i = 0; i < stacks.Count; i++)
            {
                //Create New Stack
                Stack<Folder> newstack = new Stack<Folder>();

                //Get Stack at Index i from stacks and convert it to list
                List<Tuple<int, int, string, string>> stack = stacks[i].ToList();

                //Loop through elements in stack
                for (int j = 0; j < stack.Count; j++)
                {
                    //Add Current Looped Element to newstack
                    newstack.Push(new Folder() { Id = stack[j].Item1, OwnerId = stack[j].Item2, Name = stack[j].Item3, FolderType = stack[j].Item4 });
                }

                //Add newstack to newstacks
                newstacks.Push(newstack);
            }

            //Return newstacks
            return newstacks;
        }
        #endregion Extensions
    }
}
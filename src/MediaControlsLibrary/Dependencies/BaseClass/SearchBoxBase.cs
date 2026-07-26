using System;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;
using MediaControlsLibrary.Models;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using System.Threading.Tasks;

namespace MediaControlsLibrary.Dependencies
{
    public class SearchBoxBase : HeaderedContentControl
    {
        #region Variables
        // Button
        // =========================================================
        // =========================================================
        // =========================================================
        private const string str_Button = "PART_Button";
        private static Button PART_Button { get; set; }




        // Cover
        // =========================================================
        // =========================================================
        // =========================================================
        private const string str_Cover = "PART_Image";
        private static ImageBrush PART_Cover { get; set; }




        // Title
        // =========================================================
        // =========================================================
        // =========================================================
        private const string str_Title = "PART_Title";
        private static TextBlock PART_Title { get; set; }




        // Platforms
        // =========================================================
        // =========================================================
        // =========================================================
        private const string str_Platforms = "PART_Platforms";
        private static TextBlock PART_Platforms { get; set; }




        // Type
        // =========================================================
        // =========================================================
        // =========================================================
        private const string str_Type = "PART_Type";
        private static TextBlock PART_Type { get; set; }




        // Popup
        // =========================================================
        // =========================================================
        // =========================================================
        private const string str_Popup = "PART_Popup";
        private static Popup PART_Popup { get; set; }




        // Search Text
        // =========================================================
        // =========================================================
        // =========================================================
        private const string str_SearchText = "PART_SearchText";
        private static TextBox PART_SearchText { get; set; }




        // ListBox
        // =========================================================
        // =========================================================
        // =========================================================
        private const string str_ListBox = "PART_ListBox";
        private static ListBox PART_ListBox { get; set; }




        // Other
        // =========================================================
        // =========================================================
        // =========================================================
        private bool isOpen = false;
        private bool isWindowDeactivated = false;
        private Timer ErrorTimer;
        private Timer SearchChangedTimer;
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;
        #endregion Variables





        #region Fields
        // GUI
        // ====================================================
        // ====================================================
        // ====================================================
        public static readonly DependencyProperty PopupHeightProperty = DependencyProperty.Register(nameof(PopupHeight), typeof(double), typeof(SearchBoxBase), new PropertyMetadata(400.0));
        public static readonly DependencyProperty LabelPlaceholderProperty = DependencyProperty.Register(nameof(LabelPlaceholder), typeof(string), typeof(SearchBoxBase), new PropertyMetadata("Select a Title..."));
        public static readonly DependencyProperty SearchPlaceholderProperty = DependencyProperty.Register(nameof(SearchPlaceholder), typeof(string), typeof(SearchBoxBase), new PropertyMetadata("Enter Search Text..."));




        // Functional
        // ====================================================
        // ====================================================
        // ====================================================
        // Selection
        // ====================================================
        // ====================================================
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(SearchBoxBase), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty CoverProperty = DependencyProperty.Register(nameof(Cover), typeof(string), typeof(SearchBoxBase), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty PlatformsProperty = DependencyProperty.Register(nameof(Platforms), typeof(List<string>), typeof(SearchBoxBase), new PropertyMetadata(new List<string>()));
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(nameof(Type), typeof(string), typeof(SearchBoxBase), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty DefaultLinkProperty = DependencyProperty.Register(nameof(DefaultLink), typeof(string), typeof(SearchBoxBase), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty IMDBLinkProperty = DependencyProperty.Register(nameof(IMDBLink), typeof(string), typeof(SearchBoxBase), new PropertyMetadata(default(string)));



        // Search
        // ====================================================
        // ====================================================
        // ====================================================
        public static readonly DependencyProperty SearchProperty = DependencyProperty.Register(nameof(Search), typeof(string), typeof(SearchBoxBase), new PropertyMetadata(default(string)));
        public static readonly RoutedEvent SearchChangedEvent = EventManager.RegisterRoutedEvent(nameof(SearchChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SearchBoxBase));
        public static readonly DependencyProperty isLoadingProperty = DependencyProperty.Register(nameof(isLoading), typeof(bool), typeof(SearchBoxBase), new PropertyMetadata(false));
        public static readonly DependencyProperty SearchErrorProperty = DependencyProperty.Register(nameof(SearchError), typeof(string), typeof(SearchBoxBase), new PropertyMetadata("An error occured during the search..."));
        public static readonly DependencyProperty isErrorProperty = DependencyProperty.Register(nameof(isError), typeof(bool), typeof(SearchBoxBase), new PropertyMetadata(false, OnErrorToggledAsync));
        public static readonly DependencyProperty ErrorSpanProperty = DependencyProperty.Register(nameof(ErrorSpan), typeof(int), typeof(SearchBoxBase), new PropertyMetadata(5));


        // Other
        // ====================================================
        // ====================================================
        // ====================================================
        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(nameof(Items), typeof(ObservableCollection<object>), typeof(SearchBoxBase), new PropertyMetadata(new ObservableCollection<object>()));
        public static readonly DependencyProperty DefaultCoverProperty = DependencyProperty.Register(nameof(DefaultCover), typeof(ImageSource), typeof(SearchBoxBase), new PropertyMetadata(default(ImageSource)));
        #endregion Fields





        #region Properties
        #region GUI
        // Popup Height
        // =========================================================
        // =========================================================
        public double PopupHeight
        {
            get => (double)GetValue(PopupHeightProperty);
            set => SetValue(PopupHeightProperty, value);
        }



        // Label Placeholder
        // =========================================================
        // =========================================================
        public string LabelPlaceholder
        {
            get => (string)GetValue(LabelPlaceholderProperty);
            set => SetValue(LabelPlaceholderProperty, value);
        }



        // Search Placeholder
        // =========================================================
        // =========================================================
        public string SearchPlaceholder
        {
            get => (string)GetValue(SearchPlaceholderProperty);
            set => SetValue(SearchPlaceholderProperty, value);
        }
        #endregion GUI




        #region Functional
        // Selection
        // =========================================================
        // =========================================================
        // Title
        // =========================================================
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }


        // Cover
        // =========================================================
        public string Cover
        {
            get => (string)GetValue(CoverProperty);
            set => SetValue(CoverProperty, value);
        }


        // Platforms
        // =========================================================
        public List<string> Platforms
        {
            get => (List<string>)GetValue(PlatformsProperty);
            set => SetValue(PlatformsProperty, value);
        }


        // Type
        // =========================================================
        public string Type
        {
            get => (string)GetValue(TypeProperty);
            set => SetValue(TypeProperty, value);
        }


        // Default Link
        // =========================================================
        public string DefaultLink
        {
            get => (string)GetValue(DefaultLinkProperty);
            set => SetValue(DefaultLinkProperty, value);
        }


        // IMDB Link
        // =========================================================
        public string IMDBLink
        {
            get => (string)GetValue(IMDBLinkProperty);
            set => SetValue(IMDBLinkProperty, value);
        }



        // Search
        // =========================================================
        // =========================================================
        // Search
        // =========================================================
        public string Search
        {
            get => (string)GetValue(SearchProperty);
            set => SetValue(SearchProperty, value);
        }


        // Search Changed
        // =========================================================
        public event RoutedEventHandler SearchChanged
        {
            add { AddHandler(SearchChangedEvent, value); }
            remove { RemoveHandler(SearchChangedEvent, value); }
        }


        // is Loading
        // =========================================================
        public bool isLoading
        {
            get => (bool)GetValue(isLoadingProperty);
            set => SetValue(isLoadingProperty, value);
        }


        // Search Error
        // =========================================================
        public string SearchError
        {
            get => (string)GetValue(SearchErrorProperty);
            set => SetValue(SearchErrorProperty, value);
        }


        // is Error
        // =========================================================
        public bool isError
        {
            get => (bool)GetValue(isErrorProperty);
            set => SetValue(isErrorProperty, value);
        }


        // Error Span
        // =========================================================
        public int ErrorSpan
        {
            get => (int)GetValue(ErrorSpanProperty);
            set => SetValue(ErrorSpanProperty, value);
        }



        // Other
        // ==========================================================
        // ==========================================================
        // Search
        // =========================================================
        public ObservableCollection<object> Items
        {
            get => (ObservableCollection<Object>)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }


        // Default Cover
        // =========================================================
        public ImageSource DefaultCover
        {
            get => (ImageSource)GetValue(DefaultCoverProperty);
            set => SetValue(DefaultCoverProperty, value);
        }
        #endregion Functional
        #endregion Properties





        #region Event Handlers
        // Search
        // ====================================================
        // ====================================================
        // ====================================================
        // Search Text
        // ====================================================
        // ====================================================
        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Validate Timer State
            if (!SearchChangedTimer.Enabled)
            {
                //Start Timer
                SearchChangedTimer.Start();
            }
            else
            {
                //Stop Timer
                SearchChangedTimer.Stop();

                //Start Timer
                SearchChangedTimer.Start();
            }
        }



        // Search Changed
        // ====================================================
        // ====================================================
        private void SearchChangedTimer_Elapsed(object sender, ElapsedEventArgs args)
        {
            //Stop Timer
            SearchChangedTimer.Stop();

            //Run Tasks on UI Thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                //Set Search to PART_SearchText Text
                Search = PART_SearchText.Text;

                //Validate Search Text
                if (!string.IsNullOrEmpty(Search))
                {
                    //Initialize RoutedEventArgs Object
                    RoutedEventArgs newEventArgs = new RoutedEventArgs(SearchChangedEvent);

                    //Raise RoutedEventArgs Object Event
                    RaiseEvent(newEventArgs);
                }
                else
                {
                    //Clear Search Box
                    Clear();
                }
            });
        }




        // Error
        // ====================================================
        // ====================================================
        // ====================================================
        private static async void OnErrorToggledAsync(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            //Get Search Box
            SearchBoxBase searchbox = (SearchBoxBase)obj;

            //Check if an Error Occured
            if (searchbox.isError)
            {
                //Wait X Seconds
                await Task.Delay(searchbox.ErrorSpan * 1000);

                //Set isError to False
                searchbox.isError = false;
            }
        }
        #endregion Event Handlers





        // Apply Template
        // =========================================================
        // =========================================================
        public override void OnApplyTemplate()
        {
            //Fire When the Element's Template is Applied
            base.OnApplyTemplate();

            //Get Elements
            PART_Button = (Button)Template.FindName(str_Button, this);
            PART_Cover = (ImageBrush)Template.FindName(str_Cover, this);
            PART_Title = (TextBlock)Template.FindName(str_Title, this);
            PART_Platforms = (TextBlock)Template.FindName(str_Platforms, this);
            PART_Type = (TextBlock)Template.FindName(str_Type, this);
            PART_Popup = (Popup)Template.FindName(str_Popup, this);
            PART_SearchText = (TextBox)Template.FindName(str_SearchText, this);
            PART_ListBox = (ListBox)Template.FindName(str_ListBox, this);

            //Set Default Cover Image
            SetImage(DefaultCover.ToString());

            //Set Event Handlers
            Application.Current.Deactivated += Window_Deactivated;
            LostFocus += UserControl_FocusLost;
            PART_Button.Click += PART_Button_Click;
            PART_SearchText.TextChanged += Search_TextChanged;
            PART_ListBox.SelectionChanged += PART_ListBox_SelectionChanged;

            //Set Search Changed Timer
            this.SearchChangedTimer = new Timer(1000);
            this.SearchChangedTimer.Elapsed += SearchChangedTimer_Elapsed;
        }





        #region Event Handlers
        // Window
        // =========================================================
        // =========================================================
        // =========================================================
        private void Window_Deactivated(object sender, EventArgs e)
        {
            //Check if the Main Window is Not Set to Top Most
            if (!Application.Current.MainWindow.Topmost && PART_Popup.IsOpen)
            {
                //Toggle Popup
                TogglePopup();
            }

            //Check if the Search Box Popup is Open
            if (PART_Popup.IsOpen)
            {
                //Set isWindowDeactivated to True
                isWindowDeactivated = true;
            }
        }




        // User Control
        // =========================================================
        // =========================================================
        // =========================================================
        private void UserControl_FocusLost(object sender, RoutedEventArgs e)
        {
            //Validate Focus Lost
            if(Application.Current.MainWindow.Topmost && PART_Popup.IsOpen)
            {
                //Set isWindowDeactivated to False
                isWindowDeactivated = false;
            }
            else if (!IsKeyboardFocusWithin)
            {
                //Toggle Popup
                TogglePopup();

                //Set isWindowDeactivated to False
                isWindowDeactivated = false;
            }
        }




        // Button
        // =========================================================
        // =========================================================
        // =========================================================
        private void PART_Button_Click(object sender, RoutedEventArgs e)
        {
            //Toggle Popup
            TogglePopup();
        }




        // ListBox
        // =========================================================
        // =========================================================
        // =========================================================
        private void PART_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Selection Change
            SelectionChange(PART_ListBox.SelectedItem);

            //Invoke Selection Changed
            SelectionChanged.Invoke(this, e);
        }
        #endregion Event Handlers





        #region Methods
        // Selection Changed
        // =========================================================
        // =========================================================
        // =========================================================
        private void SelectionChange(object item)
        {
            //Check if an Item has Been Selected
            if (item != null)
            {
                //Validate Search Item Type
                if (item.ToString().Contains(nameof(MovieSearch)))
                {
                    //Convert item Object to MovieSearch Object
                    MovieSearch movie = (MovieSearch)item;

                    //Set Selection
                    Select(movie.Name, movie.CoverImage, movie.MetacriticLink, movie.IMDBLink);
                }
                else
                {
                    //Convert item Object to GameSearch Object
                    GameSearch game = (GameSearch)item;

                    //Set Selection
                    Select(game.Name, game.CoverImage, game.Platforms, game.Type, game.IGDBLink);
                }

                //Toggle Popup
                TogglePopup();
            }
        }




        // Items
        // =========================================================
        // =========================================================
        // =========================================================
        public void Add(string name, string cover, string metacriticlink, string imdblink) { Items.Add(new MovieSearch() { Name = name, CoverImage = cover, MetacriticLink = metacriticlink, IMDBLink = imdblink }); }

        public void Add(string igdblink, string name, string cover, string type, List<string> platforms) { Items.Add(new GameSearch() { IGDBLink = igdblink, Name = name, CoverImage = cover, Type = type, Platforms = platforms }); }

        public void Clear(bool isItems = false) {
            //Validate Search Box Results
            if (Items.Count > 0)
            {
                //Clear Items
                Items.Clear();
            }

            //Validate UI Elements
            if (PART_Cover != null && !isItems)
            {
                //Set Default Cover Image
                SetImage(DefaultCover.ToString());

                //Unset Search Text
                PART_SearchText.Text = string.Empty;

                //Unset Platforms
                PART_Platforms.Text = string.Empty;
            }
        }
        #endregion Methods





        #region Extensions
        // Cover
        // =========================================================
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

            //Set Cover
            PART_Cover.ImageSource = source;
        }




        // Platforms
        // =========================================================
        // =========================================================
        // =========================================================
        private void SetPlatforms(List<string> platforms)
        {
            //Variables
            string value = "";

            //Check if the Platforms List Contains Any Platforms
            if (platforms != null && platforms.Count > 0)
            {
                //Loop through Platforms List
                for (int i = 0; i < platforms.Count; i++)
                {
                    //Check if the Current Looped Platform Value is Not the Last Platform Value withint the Platforms List 
                    if (i < platforms.Count - 1)
                    {
                        //Add Current Looped Platform to value String
                        value += $"{platforms[i]}, ";
                    }
                    else
                    {
                        //Add Current Looped Platform to value String
                        value += platforms[i];
                    }
                }

                //Display Platforms
                PART_Platforms.Text = value;

                //Show Platforms TextBlock
                PART_Platforms.Visibility = Visibility.Visible;
            }
            else
            {
                //Collapse Platforms TextBlock
                PART_Platforms.Visibility = Visibility.Collapsed;
            }
        }




        // Popup
        // =========================================================
        // =========================================================
        // =========================================================
        private void TogglePopup()
        {
            //Toggle Popup
            PART_Popup.IsOpen = !isOpen;

            //Toggle IsOpen Variable
            isOpen = !isOpen;

            //Set Focus on Search Textbox
            PART_SearchText.Focus();
        }




        // Select
        // =========================================================
        // =========================================================
        // =========================================================
        public void Select(string title, string cover, string defaultlink, string imdblink)
        {
            //Set Title
            Title = title;

            //Set Cover
            SetImage(cover);

            //Set Links
            DefaultLink = defaultlink;
            IMDBLink = imdblink;
        }

        public void Select(string title, string cover, List<string> platforms, string type, string defaultlink)
        {
            //Set Selected Values
            Title = title;
            Platforms = platforms;
            Type = type;

            //
            PART_Cover = (ImageBrush)Template.FindName(str_Cover, this);

            //Set Cover
            SetImage(cover);

            //Set Platforms
            SetPlatforms(platforms);

            //Set Links
            DefaultLink = defaultlink;
            IMDBLink = string.Empty;
        }




        // Deselect
        // =========================================================
        // =========================================================
        // =========================================================
        public void Deselect()
        {
            //Validate Selected Item
            if (PART_ListBox != null)
            {
                //Deselect ListBox Item
                PART_ListBox.SelectedItem = null;

                //Unset Selected Item
                Title = string.Empty;
                Cover = string.Empty;
                Platforms = new List<string>();
                Type = string.Empty;
                DefaultLink = string.Empty;
                IMDBLink = string.Empty;
            }
        }
        #endregion Extensions
    }
}
using System.IO;
using System.Windows;
using MediaControlsLibrary;
using System.Threading.Tasks;
using Media_Manager.ViewModels;
using System.Collections.Generic;
using Media_Manager.Metadata;
using System;
using System.Threading;
using Media_Manager.Data;
using Microsoft.Win32;

namespace Media_Manager
{
    public partial class MainWindow : Window
    {
        #region Variables
        // Menu
        // =========================================
        // =========================================
        private NavigationViewItem selectedNavItem = null;


        // Pages
        // =========================================
        // =========================================
        private const string NavItemPrefix = "NavItem";
        private readonly Dictionary<string, object> Pages = new Dictionary<string, object>()
        {
            { "Movies" + NavItemPrefix, new MoviesViewModel() },
            { "TVShows" + NavItemPrefix, new TVShowsViewModel() },
            { "Videos" + NavItemPrefix, new VideosViewModel() },
            { "Pictures" + NavItemPrefix, new PicturesViewModel() },
            { "Music" + NavItemPrefix, new MusicViewModel() },
            { "Games" + NavItemPrefix, new GamesViewModel() },
            { "VideoPlayer" + NavItemPrefix, new VideoPlayerViewModel() },
            { "PictureGallery" + NavItemPrefix, new PictureGalleryViewModel() }
        };
        #endregion Variables



        // Constructor
        // =========================================
        // =========================================
        public MainWindow()
        {
            InitializeComponent();

            Setup();
        }



        // Setup
        // =========================================
        // =========================================
        private void Setup()
        {
            //Initialize Overlays
            ToggleState.InitializeOverlay(Overlay, LoadingPanel);

            //Set Default Navigation Menu Item
            SelectNavItem((NavigationViewItem)NavigationMenu.FindName("Movies" + NavItemPrefix));

            //Set Default View Model
            DataContext = new MoviesViewModel();

            //Initialize Application's Local Data Path
            string applicationName = typeof(MainWindow).Assembly.GetName().Name;
            string dataDirectoryOverride = System.Environment.GetEnvironmentVariable("MEDIA_MANAGER_DATA_DIRECTORY");
            string localdata = string.IsNullOrWhiteSpace(dataDirectoryOverride)
                ? Path.Combine(
                    System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData),
                    applicationName)
                : Path.GetFullPath(dataDirectoryOverride);

            ApplicationLog.Initialize(localdata);
            LibraryDataService.Initialize(localdata);
            bool recovered = LibraryDataService.RecoverDatabaseIfRequired();

            //Initialize metadata providers and their local encrypted settings/cache.
            MetadataService.Initialize(localdata);

            //Initialize Database
            Database.Initialize(localdata);
            if (string.Equals(
                Environment.GetEnvironmentVariable("MEDIA_MANAGER_DEMO_MODE"),
                "1",
                StringComparison.Ordinal))
            {
                LibraryDataService.EnsureDemoLibrary();
            }

            try
            {
                LibraryDataService.CreateAutomaticBackupIfDue();
            }
            catch (Exception exception)
            {
                ApplicationLog.Error(
                    "The automatic startup backup could not be created.",
                    exception);
            }

            //Initialize Save Locations
            Properties.Settings.Default.Movies = CreateSaveLocation(localdata + @"\Images\Movie Covers\");
            Properties.Settings.Default.TVShows = CreateSaveLocation(localdata + @"\Images\TV Show Covers\");
            Properties.Settings.Default.Seasons = CreateSaveLocation(localdata + @"\Images\Season Covers\");
            Properties.Settings.Default.Episodes = CreateSaveLocation(localdata + @"\Images\Episode Covers\");
            Properties.Settings.Default.Videos = CreateSaveLocation(localdata + @"\Images\Video Preview\");
            Properties.Settings.Default.Pictures = CreateSaveLocation(localdata + @"\Images\Image Preview\");
            Properties.Settings.Default.Music = CreateSaveLocation(localdata + @"\Images\Music Covers\");
            Properties.Settings.Default.Games = CreateSaveLocation(localdata + @"\Images\Game Covers\");
            txtDataStatus.Text = recovered
                ? "The library database was recovered from a verified backup."
                : "Automatic daily backups are enabled.";
        }



        #region Event Handlers
        // Navigation
        // =========================================
        // =========================================
        private void Navigation_Click(object sender, RoutedEventArgs e)
        {
            //Set Selected Navigation Item
            SelectNavItem((NavigationViewItem)sender);

            //Load Associated View
            LoadView(((NavigationViewItem)sender).Name);
        }


        // Frame
        // =========================================
        // =========================================
        private void Frame_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //Show Loading Panel
            ShowLoadingPanelAsync();
        }


        // Save Locations
        // =========================================
        // =========================================
        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            //Show Browse Locations Panel
            BrowseLocationsPanel.Visibility = Visibility.Visible;

            //Load Browse Locations
            LoadBrowseLocations();
        }

        private void btnProviders_Click(object sender, RoutedEventArgs e)
        {
            BrowseLocationsPanel.Visibility = Visibility.Collapsed;
            ProviderSettingsPanel.Visibility = Visibility.Visible;
            LoadProviderStatus();
        }

        private void btnProviderApply_Click(object sender, RoutedEventArgs e)
        {
            MetadataService.SaveSettings(
                pbTmdbAccessToken.Password,
                $"{tbIgdbClientId.Content}",
                pbIgdbClientSecret.Password);
            pbTmdbAccessToken.Clear();
            pbIgdbClientSecret.Clear();
            tbIgdbClientId.Content = string.Empty;
            LoadProviderStatus();
        }

        private void btnProviderBack_Click(object sender, RoutedEventArgs e)
        {
            ProviderSettingsPanel.Visibility = Visibility.Collapsed;
            BrowseLocationsPanel.Visibility = Visibility.Visible;
        }

        private async void btnBackupLibrary_Click(
            object sender,
            RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Title = "Back Up Media Manager Library",
                Filter = "Media Manager Backup (*.mmbak)|*.mmbak",
                DefaultExt = ".mmbak",
                AddExtension = true,
                FileName =
                    $"MediaManager-{DateTime.Now:yyyy-MM-dd-HHmm}.mmbak"
            };
            if (dialog.ShowDialog(this) != true)
            {
                return;
            }

            ToggleState.Loading(true);
            try
            {
                await Task.Run(
                    () => LibraryDataService.CreateBackup(dialog.FileName));
                txtDataStatus.Text =
                    $"Backup completed: {dialog.FileName}";
                CustomMessageBox.ShowOK(
                    "The library database and managed cover images were backed up successfully. Provider credentials and logs were not included.",
                    "Backup Complete",
                    "OK",
                    MessageBoxImage.Information);
            }
            catch (Exception exception)
            {
                ShowDataError("The backup could not be created.", exception);
            }
            finally
            {
                ToggleState.Loading(false);
            }
        }

        private async void btnRestoreLibrary_Click(
            object sender,
            RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Restore Media Manager Library",
                Filter = "Media Manager Backup (*.mmbak)|*.mmbak",
                Multiselect = false
            };
            if (dialog.ShowDialog(this) != true)
            {
                return;
            }

            MessageBoxResult confirmation = CustomMessageBox.ShowYesNo(
                "Restoring replaces the current library database and managed cover images. A safety backup will be created first. Continue?",
                "Restore Library",
                "Restore",
                "Cancel",
                MessageBoxImage.Warning);
            if (confirmation != MessageBoxResult.Yes)
            {
                return;
            }

            ToggleState.Loading(true);
            try
            {
                await Task.Run(
                    () => LibraryDataService.RestoreBackup(dialog.FileName));
                CustomMessageBox.ShowOK(
                    "Restore completed successfully. Media Manager will now close so the restored library can be loaded cleanly.",
                    "Restore Complete",
                    "Close",
                    MessageBoxImage.Information);
                Application.Current.Shutdown();
            }
            catch (Exception exception)
            {
                ShowDataError("The backup could not be restored.", exception);
                ToggleState.Loading(false);
            }
        }

        private async void btnExportCatalog_Click(
            object sender,
            RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Title = "Export Media Manager Catalog with Redacted Paths",
                Filter = "JSON Catalog (*.json)|*.json",
                DefaultExt = ".json",
                AddExtension = true,
                FileName =
                    $"MediaManager-catalog-{DateTime.Now:yyyy-MM-dd}.json"
            };
            if (dialog.ShowDialog(this) != true)
            {
                return;
            }

            ToggleState.Loading(true);
            try
            {
                await Task.Run(
                    () => LibraryDataService.ExportCatalog(
                        dialog.FileName,
                        false));
                txtDataStatus.Text =
                    $"Path-redacted catalog exported: {dialog.FileName}";
                CustomMessageBox.ShowOK(
                    "Catalog export completed. Filesystem paths were replaced with portable sample references. Media titles and metadata remain in the export.",
                    "Export Complete",
                    "OK",
                    MessageBoxImage.Information);
            }
            catch (Exception exception)
            {
                ShowDataError("The catalog could not be exported.", exception);
            }
            finally
            {
                ToggleState.Loading(false);
            }
        }

        private async void btnCheckLibrary_Click(
            object sender,
            RoutedEventArgs e)
        {
            ToggleState.Loading(true);
            try
            {
                LibraryHealthReport report = await Task.Run(
                    () => LibraryDataService.CheckLibrary(
                        CancellationToken.None));
                txtDataStatus.Text = report.Summary;
                CustomMessageBox.ShowOK(
                    report.Summary
                    + (report.IsHealthy
                        ? "\nNo duplicate or missing paths were found."
                        : "\nUnavailable items remain in the library and can be edited or removed without crashing the app."),
                    report.IsHealthy
                        ? "Library Healthy"
                        : "Library Check Complete",
                    "OK",
                    report.IsHealthy
                        ? MessageBoxImage.Information
                        : MessageBoxImage.Warning);
            }
            catch (Exception exception)
            {
                ShowDataError(
                    "The library health check could not be completed.",
                    exception);
            }
            finally
            {
                ToggleState.Loading(false);
            }
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            //Hide Browse Locations Panel
            BrowseLocationsPanel.Visibility = Visibility.Collapsed;

            //Save Browse Locations
            SaveBrowseLocations();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            //Hide Browse Locations Panel
            BrowseLocationsPanel.Visibility = Visibility.Collapsed;
        }
        #endregion Event Handlers



        #region Methods
        // Navigation
        // =========================================
        // =========================================
        private void SelectNavItem(NavigationViewItem item)
        {
            //Check if the selectedNavItem Object is not Null
            if (selectedNavItem != null)
            {
                //Set selectedNavItem's IsSelected Value to False
                selectedNavItem.IsSelected = false;
            }

            //Set selectedNavItem to Passed Item Variable
            selectedNavItem = item;

            //Set selectedNavItem's IsSelected Value to True
            selectedNavItem.IsSelected = true;
        }

        private void LoadView(string name)
        {
            //Get Page From Pages Dictionary
            Pages.TryGetValue(name, out object page);

            //Set Page as DataContext
            DataContext = page;
        }


        // Frame
        // =========================================
        // =========================================
        private async void ShowLoadingPanelAsync()
        {
            //Delay Task by 15 Milliseconds
            await Task.Delay(15);

            //Get Frame Name
            string name = string.IsNullOrEmpty($"{Frame.Content}") ? "empty" : Frame.Content.ToString().Replace("Media_Manager.Views.", "");

            //Check if Frame Content is Not Set to PictureGalleryView and VideoPlayerView
            if (name != "PictureGalleryView" && name != "VideoPlayerView")
            {
                //Show Loading Panel
                ToggleState.Loading(true);
            }
        }


        // Browse Locations
        // =========================================
        // =========================================
        private void LoadBrowseLocations()
        {
            //Load Browse Location Values
            odMovies.Content = Properties.Settings.Default.MovieBrowse;
            odTVShows.Content = Properties.Settings.Default.TVShowBrowse;
            odVideos.Content = Properties.Settings.Default.VideoBrowse;
            odPictures.Content = Properties.Settings.Default.PictureBrowse;
            odMusic.Content = Properties.Settings.Default.MusicBrowse;
            odSongCover.Content = Properties.Settings.Default.SongCoverBrowse;
            odGames.Content = Properties.Settings.Default.GameBrowse;
        }

        private void SaveBrowseLocations()
        {
            //Save Browse Location Values
            Properties.Settings.Default.MovieBrowse = $"{odMovies.Content}";
            Properties.Settings.Default.TVShowBrowse = $"{odTVShows.Content}";
            Properties.Settings.Default.VideoBrowse = $"{odVideos.Content}";
            Properties.Settings.Default.PictureBrowse = $"{odPictures.Content}";
            Properties.Settings.Default.MusicBrowse = $"{odMusic.Content}";
            Properties.Settings.Default.SongCoverBrowse = $"{odSongCover.Content}";
            Properties.Settings.Default.GameBrowse = $"{odGames.Content}";
            Properties.Settings.Default.Save();
        }

        private void LoadProviderStatus()
        {
            MetadataProviderStatus status = MetadataService.GetStatus();
            txtProviderStatus.Text =
                $"TMDB: {(status.TmdbConfigured ? "configured" : "not configured")} ({status.TmdbSource})\n"
                + $"IGDB: {(status.IgdbConfigured ? "configured" : "not configured")} ({status.IgdbSource})\n"
                + "Leave a secret field blank to keep its current saved value.";
        }


        // Save Locations
        // =========================================
        // =========================================
        private string CreateSaveLocation(string saveLocation)
        {
            if (!Directory.Exists(saveLocation))
            {
                Directory.CreateDirectory(saveLocation);
            }

            return saveLocation;
        }

        private void ShowDataError(string message, Exception exception)
        {
            ApplicationLog.Error(message, exception);
            txtDataStatus.Text = message;
            CustomMessageBox.ShowOK(
                message + "\n\n" + exception.GetBaseException().Message,
                "Data Error",
                "OK",
                MessageBoxImage.Error);
        }
        #endregion Methods
    }
}

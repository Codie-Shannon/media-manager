using System.Windows;
using MediaControlsLibrary;
using System.Collections.Generic;

namespace MediaControlsTester
{
    /// <summary>
    /// Interaction logic for MainWindow1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //Movies
            sbTitle.Add("Synthetic Speedway", "Image.jpg", "https://example.invalid/metadata/synthetic-speedway", "https://example.invalid/title/synthetic-speedway");
            sbTitle.Add("Local Adventure", "Image.jpg", "https://example.invalid/metadata/local-adventure", "https://example.invalid/title/local-adventure");

            sbTitle.Add("https://example.invalid/games/synthetic-speedway", "Synthetic Speedway", "Image.jpg", "Main Game", new List<string>() { "Desktop", "Console", "Handheld" });
            sbTitle.Add("https://example.invalid/games/sample-expansion", "Sample Expansion", "cover.jpg", "Expansion", null);

            sbTitle.isError = true;
            //sbTitle.isLoading = true;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddPanel.Visibility = Visibility.Visible;

            optFolderBrowserDialog.Clear();

            optFolderBrowserDialog.AddFolder(-1, -2, "Base");
            optFolderBrowserDialog.AddFolder(0, -1, "Main");
            optFolderBrowserDialog.AddFolder(1, 0, "Folder 1");
            optFolderBrowserDialog.AddFolder(2, 1, "Folder 2");
        }

        private void btnAddBack_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Owner ID: {fbdSaveLocation.Id}\nDB Name: {fbdSaveLocation.DBName}");

            AddPanel.Visibility = Visibility.Collapsed;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = CustomMessageBox.ShowYesNoCancel("The picture has already been added to the database. Are you sure you want continue with this procedure?", "WARNING", "Yes Button", "No Button", "Cancel Button", MessageBoxImage.Warning);

            System.Diagnostics.Debug.WriteLine($"Result: {result}");
        }

        private void ContextMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NavigationBar_Back(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Back Click 2");
        }

        private void NavigationBar_Forward(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Forward Click 2");
        }

        private void btnAdd_Click_1(object sender, RoutedEventArgs e)
        {
            optFolderBrowserDialog.Clear();

            optFolderBrowserDialog.AddFolder(0, -1, "Main");
            optFolderBrowserDialog.AddFolder(1, 0, "Folder 1");
            optFolderBrowserDialog.AddFolder(2, 1, "Folder 2");
        }

        private void sbTitle_SearchChanged(object sender, RoutedEventArgs e)
        {
            System.Console.WriteLine($"Title: {sbTitle.Search}");
        }

        private void sbTitle_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            System.Console.WriteLine($"Selection Changed:\nSelection Title: {sbTitle.Title}\nIMDB Link: {sbTitle.IMDBLink}\nMetacritic Link: {sbTitle.DefaultLink}");
        }

        private void odPath_ClearClick(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("\nClear Click\n");
        }

        private void optNumericBox_Click(object sender, RoutedEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine($"{((FrameworkElement)((FrameworkElement)((FrameworkElement)((FrameworkElement)sender).Parent).Parent).Parent).Parent.GetType()}");
        }

        private void btnProcessItem_Click(object sender, RoutedEventArgs e)
        {
            //nbSeason.Clear();

            //System.Diagnostics.Debug.WriteLine($"Clear {nbSeason.Value}");
        }
    }
}

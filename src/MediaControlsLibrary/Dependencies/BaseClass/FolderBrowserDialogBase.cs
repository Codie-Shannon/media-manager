using System.Windows;
using System.Windows.Controls;

namespace MediaControlsLibrary.Dependencies
{
    public class FolderBrowserDialogBase : HeaderedContentControl
    {
        #region Fields
        // Folder Browser Dialog UI
        // ====================================================
        // ====================================================
        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register(nameof(Placeholder), typeof(string), typeof(FolderBrowserDialogBase), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty ButtonContentProperty = DependencyProperty.Register(nameof(ButtonContent), typeof(string), typeof(FolderBrowserDialogBase), new PropertyMetadata(default(string)));


        // Folder Browser
        // ====================================================
        // ====================================================
        // ====================================================
        public static readonly DependencyProperty WindowCaptionProperty = DependencyProperty.Register(nameof(WindowCaption), typeof(string), typeof(FolderBrowserDialogBase), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty InitialFolderIDProperty = DependencyProperty.Register(nameof(InitialFolderID), typeof(int), typeof(FolderBrowserDialogBase), new PropertyMetadata(default(int)));
        public static readonly DependencyProperty EditSelectedIdProperty = DependencyProperty.Register(nameof(EditSelectedId), typeof(int), typeof(FolderBrowserDialogBase), new PropertyMetadata(default(int)));


        // Folder Browser Dialog Functionality
        // ====================================================
        // ====================================================
        public static readonly DependencyProperty IdProperty = DependencyProperty.Register(nameof(Id), typeof(int), typeof(FolderBrowserDialogBase), new PropertyMetadata(default(int)));
        public static readonly DependencyProperty DBNameProperty = DependencyProperty.Register(nameof(DBName), typeof(string), typeof(FolderBrowserDialogBase), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty FolderPathProperty = DependencyProperty.Register(nameof(FolderPath), typeof(string), typeof(FolderBrowserDialogBase), new PropertyMetadata(default(string), OnFolderPathChanged));
        #endregion Fields



        #region Properties
        // Folder Browser Dialog UI
        // ====================================================
        // ====================================================
        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public string ButtonContent
        {
            get => (string)GetValue(ButtonContentProperty);
            set => SetValue(ButtonContentProperty, value);
        }


        // Folder Browser
        // ====================================================
        // ====================================================
        public string WindowCaption
        {
            get => (string)GetValue(WindowCaptionProperty);
            set => SetValue(WindowCaptionProperty, value);
        }

        public int InitialFolderID
        {
            get => (int)GetValue(InitialFolderIDProperty);
            set => SetValue(InitialFolderIDProperty, value);
        }

        public int EditSelectedId
        {
            get => (int)GetValue(EditSelectedIdProperty);
            set => SetValue(EditSelectedIdProperty, value);
        }


        // Folder Browser Dialog Functionality
        // ====================================================
        // ====================================================
        public int Id
        {
            get => (int)GetValue(IdProperty);
            set => SetValue(IdProperty, value);
        }

        public string DBName
        {
            get => (string)GetValue(DBNameProperty);
            set => SetValue(DBNameProperty, value);
        }

        public string FolderPath
        {
            get => (string)GetValue(FolderPathProperty);
            set => SetValue(FolderPathProperty, value);
        }
        #endregion Properties



        // Event Handlers
        // ========================================================
        // ========================================================
        private static void OnFolderPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Convert d to Folder Browser Dialog Base
            FolderBrowserDialogBase sender = d as FolderBrowserDialogBase;

            //Set Content of Sender to it's Folder Path Property Value
            sender.Content = d.GetValue(FolderPathProperty);
        }
    }
}
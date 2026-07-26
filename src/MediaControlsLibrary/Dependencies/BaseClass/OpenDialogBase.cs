using MediaControlsLibrary.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using static MediaControlsLibrary.Types.FileTypes;
using static MediaControlsLibrary.Types.FolderPath;

namespace MediaControlsLibrary.Dependencies
{
    public class OpenDialogBase : HeaderedContentControl
    {
        #region Fields
        // UI Fields
        // ====================================================
        // ====================================================
        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register(nameof(Placeholder), typeof(string), typeof(OpenDialogBase), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty ButtonContentProperty = DependencyProperty.Register(nameof(ButtonContent), typeof(string), typeof(OpenDialogBase), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty CoverProperty = DependencyProperty.Register(nameof(Cover), typeof(string), typeof(OpenDialogBase), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty RemoveHeaderProperty = DependencyProperty.Register(nameof(RemoveHeader), typeof(string), typeof(OpenDialogBase), new PropertyMetadata(default(string)));


        // Code Fields
        // ====================================================
        // ====================================================
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(nameof(Type), typeof(FileType), typeof(OpenDialogBase), new PropertyMetadata(default(FileType)));
        public static readonly DependencyProperty IsClearProperty = DependencyProperty.Register(nameof(IsClear), typeof(bool), typeof(OpenDialogBase), new PropertyMetadata(default(bool)));
        public static readonly DependencyProperty IsMultiSelectionProperty = DependencyProperty.Register(nameof(IsMultiSelection), typeof(bool), typeof(OpenDialogBase), new PropertyMetadata(default(bool)));
        public static readonly DependencyProperty ContentsProperty = DependencyProperty.Register(nameof(Contents), typeof(ObservableCollection<OpenDialogItem>), typeof(OpenDialogBase), new PropertyMetadata(new ObservableCollection<OpenDialogItem>()));
        public static readonly DependencyProperty SelectionNameProperty = DependencyProperty.Register(nameof(SelectionName), typeof(string), typeof(OpenDialogBase), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty SelectablesProperty = DependencyProperty.Register(nameof(Selectables), typeof(string), typeof(OpenDialogBase), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty RootDirectoryProperty = DependencyProperty.Register(nameof(RootDirectory), typeof(FolderPathType), typeof(OpenDialogBase), new PropertyMetadata(FolderPathType.Desktop));
        public static readonly DependencyProperty InitialDirectoryProperty = DependencyProperty.Register(nameof(InitialDirectory), typeof(string), typeof(OpenDialogBase), new PropertyMetadata(default(string)));
        #endregion Fields



        #region Properties
        // UI Properties
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

        public string Cover
        {
            get => (string)GetValue(CoverProperty);
            set => SetValue(CoverProperty, value);
        }

        public string RemoveHeader
        {
            get => (string)GetValue(RemoveHeaderProperty);
            set => SetValue(RemoveHeaderProperty, value);
        }


        // Code Properties
        // ====================================================
        // ====================================================
        public FileType Type
        {
            get => (FileType)GetValue(TypeProperty);
            set => SetValue(TypeProperty, value);
        }

        public bool IsClear
        {
            get => (bool)GetValue(IsClearProperty);
            set => SetValue(IsClearProperty, value);
        }

        public bool IsMultiSelection
        {
            get => (bool)GetValue(IsMultiSelectionProperty);
            set => SetValue(IsMultiSelectionProperty, value);
        }

        public ObservableCollection<OpenDialogItem> Contents
        {
            get => (ObservableCollection<OpenDialogItem>)GetValue(ContentsProperty);
            set => SetValue(ContentsProperty, value);
        }

        public string SelectionName
        {
            get => (string)GetValue(SelectionNameProperty);
            set => SetValue(SelectionNameProperty, value);
        }

        public string Selectables
        {
            get => (string)GetValue(SelectablesProperty);
            set => SetValue(SelectablesProperty, value);
        }

        public FolderPathType RootDirectory
        {
            get => (FolderPathType)GetValue(RootDirectoryProperty);
            set => SetValue(RootDirectoryProperty, value);
        }

        public string InitialDirectory
        {
            get => (string)GetValue(InitialDirectoryProperty);
            set => SetValue(InitialDirectoryProperty, value);
        }
        #endregion Properties



        // Constructor
        // ====================================================
        // ====================================================
        public OpenDialogBase()
        {
            //Set Content to Empty String (Used to Show Placeholder Text)
            Content = string.Empty;
        }
    }
}
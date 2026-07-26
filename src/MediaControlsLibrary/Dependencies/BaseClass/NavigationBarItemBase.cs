using MediaControlsLibrary.Types;
using System.Windows;
using System.Windows.Controls;

namespace MediaControlsLibrary.Dependencies
{
    public class NavigationBarItemBase : Button
    {
        // Fields
        // ====================================================
        // ====================================================
        public static readonly DependencyProperty IdProperty = DependencyProperty.Register(nameof(Id), typeof(int), typeof(NavigationBarItemBase), new PropertyMetadata(default(int)));
        public static readonly DependencyProperty OwnerIdProperty = DependencyProperty.Register(nameof(OwnerId), typeof(int), typeof(NavigationBarItemBase), new PropertyMetadata(default(int)));
        public static readonly DependencyProperty DBNameProperty = DependencyProperty.Register(nameof(DBName), typeof(string), typeof(NavigationBarItemBase), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty FolderTypeProperty = DependencyProperty.Register(nameof(FolderType), typeof(string), typeof(NavigationBarItemBase), new PropertyMetadata(default(string)));


        // Properties
        // ====================================================
        // ====================================================
        public int Id
        {
            get => (int)GetValue(IdProperty);
            set => SetValue(IdProperty, value);
        }

        public int OwnerId
        {
            get => (int)GetValue(OwnerIdProperty);
            set => SetValue(OwnerIdProperty, value);
        }

        public string DBName
        {
            get => (string)GetValue(DBNameProperty);
            set => SetValue(DBNameProperty, value);
        }

        public string FolderType
        {
            get => (string)GetValue(FolderTypeProperty);
            set => SetValue(FolderTypeProperty, value);
        }
    }
}
using MediaControlsLibrary.Types;
using System.Windows;

namespace MediaControlsLibrary.Dependencies
{
    public class FolderBase : ElementBase
    {
        // Fields
        // =========================================================
        // =========================================================
        public static readonly DependencyProperty FolderTypeProperty = DependencyProperty.Register(nameof(FolderType), typeof(FolderType), typeof(ElementBase), new PropertyMetadata(default(FolderType)));
        public static readonly DependencyProperty IsTVShowProperty = DependencyProperty.Register(nameof(IsTVShow), typeof(bool), typeof(ElementBase), new PropertyMetadata(default(bool)));


        // Properties
        // =========================================================
        // =========================================================
        public FolderType FolderType
        {
            get => (FolderType)GetValue(FolderTypeProperty);
            set => SetValue(FolderTypeProperty, value);
        }

        public bool IsTVShow
        {
            get => (bool)GetValue(IsTVShowProperty);
            set => SetValue(IsTVShowProperty, value);
        }


        // Constructor
        // =========================================================
        // =========================================================
        static FolderBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FolderBase), new FrameworkPropertyMetadata(typeof(FolderBase)));
        }
    }
}
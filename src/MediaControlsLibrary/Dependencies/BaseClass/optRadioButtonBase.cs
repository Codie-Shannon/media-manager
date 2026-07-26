using MediaControlsLibrary.Types;
using System.Windows;
using System.Windows.Controls;

namespace MediaControlsLibrary.Dependencies
{
    public class optRadioButtonBase : RadioButton
    {
        // Fields
        // ====================================================
        // ====================================================
        public static readonly DependencyProperty FolderTypeProperty = DependencyProperty.Register(nameof(FolderType), typeof(FolderType), typeof(optRadioButtonBase), new PropertyMetadata(default(FolderType)));


        // Properties
        // ====================================================
        // ====================================================
        public FolderType FolderType
        {
            get => (FolderType)GetValue(FolderTypeProperty);
            set => SetValue(FolderTypeProperty, value);
        }
    }
}
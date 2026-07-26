using System.Windows;
using System.Windows.Controls;

namespace MediaControlsLibrary
{
    public class subComboBoxItemSeparator : Control
    {
        static subComboBoxItemSeparator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(subComboBoxItemSeparator), new FrameworkPropertyMetadata(typeof(subComboBoxItemSeparator)));
        }
    }
}
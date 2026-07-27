using System.Windows;
using System.Windows.Controls;

namespace Media_Manager.Controls
{
    public sealed class ModernEmptyState : Control
    {
        public static readonly DependencyProperty HeadingProperty =
            DependencyProperty.Register(
                nameof(Heading),
                typeof(string),
                typeof(ModernEmptyState),
                new PropertyMetadata("Your library is ready"));

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(
                nameof(Message),
                typeof(string),
                typeof(ModernEmptyState),
                new PropertyMetadata(
                    "Choose Add to bring local media into this library."));

        static ModernEmptyState()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ModernEmptyState),
                new FrameworkPropertyMetadata(typeof(ModernEmptyState)));
        }

        public string Heading
        {
            get => (string)GetValue(HeadingProperty);
            set => SetValue(HeadingProperty, value);
        }

        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }
    }
}

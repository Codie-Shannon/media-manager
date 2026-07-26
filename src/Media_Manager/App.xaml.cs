using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Media_Manager
{
    public partial class App : Application
    {
        public App()
        {
            //Show Splash Screen for 3 Seconds
            System.Threading.Thread.Sleep(3000);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                CreateAndShowMainWindow();
                ShutdownMode = ShutdownMode.OnMainWindowClose;
            }
            catch (Exception exception)
            {
                ShowStartupError(exception.GetBaseException());
            }
        }

        private void CreateAndShowMainWindow()
        {
            MainWindow mainWindow = new MainWindow();
            MainWindow = mainWindow;
            mainWindow.Show();
        }

        private void ShowStartupError(Exception exception)
        {
            TextBlock title = new TextBlock
            {
                Text = "Media Manager could not start",
                FontSize = 22,
                FontWeight = FontWeights.SemiBold,
                Foreground = Brushes.White
            };

            TextBlock details = new TextBlock
            {
                Text = $"Details: {exception.Message}",
                Margin = new Thickness(0, 18, 0, 0),
                TextWrapping = TextWrapping.Wrap,
                Foreground = Brushes.White
            };

            Button closeButton = new Button
            {
                Content = "Close",
                Width = 100,
                Height = 32,
                Margin = new Thickness(0, 24, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Right
            };

            StackPanel panel = new StackPanel();
            panel.Children.Add(title);
            panel.Children.Add(details);
            panel.Children.Add(closeButton);

            Window errorWindow = new Window
            {
                Title = "Media Manager Startup Error",
                Width = 560,
                Height = 250,
                MinWidth = 420,
                MinHeight = 220,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Background = new SolidColorBrush(Color.FromRgb(30, 30, 30)),
                Content = new Border
                {
                    Padding = new Thickness(28),
                    Child = panel
                }
            };

            closeButton.Click += (sender, e) => errorWindow.Close();

            MainWindow = errorWindow;
            ShutdownMode = ShutdownMode.OnMainWindowClose;
            errorWindow.Show();
        }
    }
}

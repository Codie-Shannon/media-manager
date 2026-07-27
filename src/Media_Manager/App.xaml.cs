using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Media_Manager.Data;

namespace Media_Manager
{
    public partial class App : Application
    {
        public App()
        {
            DispatcherUnhandledException += OnDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException +=
                OnUnhandledException;
            TaskScheduler.UnobservedTaskException +=
                OnUnobservedTaskException;

            //Show Splash Screen for 3 Seconds
            System.Threading.Thread.Sleep(3000);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                ConfigureDemoMode(e.Args);
                CreateAndShowMainWindow();
                ShutdownMode = ShutdownMode.OnMainWindowClose;
            }
            catch (Exception exception)
            {
                ApplicationLog.Error(
                    "Media Manager startup failed.",
                    exception.GetBaseException());
                ShowStartupError(exception.GetBaseException());
            }
        }

        private static void ConfigureDemoMode(string[] arguments)
        {
            if (arguments == null
                || !arguments.Any(value => string.Equals(
                    value,
                    "--demo",
                    StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            string demoDirectory = Path.Combine(
                Path.GetTempPath(),
                "MediaManagerDemoProfile");
            Environment.SetEnvironmentVariable(
                "MEDIA_MANAGER_DATA_DIRECTORY",
                demoDirectory,
                EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable(
                "MEDIA_MANAGER_DEMO_MODE",
                "1",
                EnvironmentVariableTarget.Process);
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

        private static void OnUnhandledException(
            object sender,
            UnhandledExceptionEventArgs e)
        {
            ApplicationLog.Error(
                "An unhandled application exception occurred.",
                e.ExceptionObject as Exception);
        }

        private static void OnUnobservedTaskException(
            object sender,
            UnobservedTaskExceptionEventArgs e)
        {
            ApplicationLog.Error(
                "An unobserved background task failed.",
                e.Exception);
            e.SetObserved();
        }

        private static void OnDispatcherUnhandledException(
            object sender,
            DispatcherUnhandledExceptionEventArgs e)
        {
            ApplicationLog.Error(
                "A user-interface operation failed.",
                e.Exception);
            MessageBox.Show(
                "Media Manager contained an unexpected error. "
                + "Your library was not deleted. Details were written to the local log.",
                "Media Manager Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            e.Handled = true;
        }
    }
}

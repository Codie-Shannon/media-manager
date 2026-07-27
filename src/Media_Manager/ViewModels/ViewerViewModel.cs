using System;
using System.Windows;
using MediaControlsLibrary;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Media_Manager
{
    public class ViewerViewModel
    {
        #region Variables
        // Timer
        // ===================================================
        // ===================================================
        private int PanelDuration = 5;
        private DispatcherTimer timer = new DispatcherTimer();


        // Elements
        // ===================================================
        // ===================================================
        private viewBar Bar;
        private Grid gBarMouseMove;
        private NavigationView NavigationMenu;
        private Grid gNavMouseMove;


        // Fullscreen
        // ===================================================
        // ===================================================
        private bool isFullscreen = false;


        // Other
        // ===================================================
        // ===================================================
        private bool isMouseMove;
        #endregion Variables



        // Constructor
        // ===================================================
        // ===================================================
        public ViewerViewModel(viewBar bar, Grid barmousemove, NavigationView navigationmenu, Grid navmousemove)
        {
            //Assign Controls
            Bar = bar;
            gBarMouseMove = barmousemove;
            NavigationMenu = navigationmenu;
            gNavMouseMove = navmousemove;


            //Setup Dispatcher Timer
            timer.Tick += new EventHandler(ClosePane_Tick);
            timer.Interval = TimeSpan.FromSeconds(PanelDuration);
        }



        #region Toggle Viewer
        // Set Viewer
        // ===================================================
        // ===================================================
        public void SetViewer()
        {
            //Get The Main Window
            Window mainWindow = Application.Current.MainWindow;

            //Hide the catalog shell so the viewer can use the complete window.
            (mainWindow.FindName("SidebarChrome") as Border).Visibility = Visibility.Collapsed;
            (mainWindow.FindName("SectionHeader") as Border).Visibility = Visibility.Collapsed;

            //Expand the frame across both shell rows and columns.
            ContentControl frame = (ContentControl)mainWindow.FindName("Frame");
            Grid.SetRow(frame, 0);
            Grid.SetRowSpan(frame, 2);
            Grid.SetColumn(frame, 0);
            Grid.SetColumnSpan(frame, 2);
        }


        // Unset Viewer
        // ===================================================
        // ===================================================
        public void UnsetViewer()
        {
            //Get The Main Window
            Window mainWindow = Application.Current.MainWindow;

            //Restore the catalog shell.
            (mainWindow.FindName("SidebarChrome") as Border).Visibility = Visibility.Visible;
            (mainWindow.FindName("SectionHeader") as Border).Visibility = Visibility.Visible;

            //Return the frame to the catalog content cell.
            ContentControl frame = (ContentControl)mainWindow.FindName("Frame");
            Grid.SetRow(frame, 1);
            Grid.SetRowSpan(frame, 1);
            Grid.SetColumn(frame, 1);
            Grid.SetColumnSpan(frame, 1);

            //Set Pane IsViewerControlOpen Boolean to False
            Pane.isViewerControlsOpen = false;
        }
        #endregion Toggle Viewer



        #region Toggle Panes
        // Methods
        // ===================================================
        // ===================================================
        // Mouse Hover
        public void MouseHover()
        {
            //Set Pane's isViewerControlsOpen Boolean to False 
            Pane.isViewerControlsOpen = false;

            //Show Panels
            Pane.Toggle(PaneToggle.Open, Bar, NavigationMenu);

            //Stop Timer
            timer.Stop();

            //Set Visibility of Mouse Move Panels to Collasped
            gBarMouseMove.Visibility = Visibility.Collapsed;
            gNavMouseMove.Visibility = Visibility.Collapsed;

            //Set isMouseMove to True
            isMouseMove = true;
        }

        // Mouse Exit
        public void MouseExit()
        {
            //Check if the Panel is Open and if Mouse Move is set to True
            if (Pane.isViewerControlsOpen == true && isMouseMove == true)
            {
                //Set Visilibty of Mouse Move Panels to Visible
                gBarMouseMove.Visibility = Visibility.Visible;
                gNavMouseMove.Visibility = Visibility.Visible;

                //Set isMouseMove to False
                isMouseMove = false;

                //Start Timer
                timer.Start();
            }
        }


        // Extensions
        // ===================================================
        // ===================================================
        // Close Pane
        private void ClosePane_Tick(object sender, EventArgs e)
        {
            //Check if the Control Panel is Open
            if (Pane.isViewerControlsOpen == true)
            {
                //Hide Panels
                Pane.Toggle(PaneToggle.Close, Bar, NavigationMenu);

                //Stop Timer
                timer.Stop();
            }
        }
        #endregion Toggle Panes



        #region Fullscreen
        // Methods
        // ===================================================
        // ===================================================
        // Change
        public void Change()
        {
            //Get The Main Window
            Window mainWindow = Application.Current.MainWindow;

            //Check if the isFullscreen boolean is set to true 
            if (isFullscreen)
            {
                //Minimize Window
                Minimize(mainWindow);

                //Set isFullscreen to False
                isFullscreen = false;
            }
            else
            {
                //Fullscreen Window
                Fullscreen(mainWindow);

                //Set isFullscreen to True
                isFullscreen = true;
            }
        }


        // Extensions
        // ===================================================
        // ===================================================
        // Fullscreen
        private void Fullscreen(Window mainWindow)
        {
            //Hide Window Before Changing Settings
            mainWindow.Visibility = Visibility.Collapsed;

            //Set Window to Fullscreen
            mainWindow.Topmost = true;
            mainWindow.WindowStyle = WindowStyle.None;
            mainWindow.ResizeMode = ResizeMode.NoResize;

            //Show Window After Changing Settings
            mainWindow.Visibility = Visibility.Visible;
        }

        // Minimize
        private void Minimize(Window mainWindow)
        {
            //Set Window to Windowed Mode
            mainWindow.Topmost = false;
            mainWindow.WindowStyle = WindowStyle.SingleBorderWindow;
            mainWindow.ResizeMode = ResizeMode.CanResize;
        }
        #endregion Fullscreen
    }
}

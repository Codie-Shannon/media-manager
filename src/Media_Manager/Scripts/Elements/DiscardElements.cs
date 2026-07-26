using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using System.Windows;
using Media_Manager.Models;
using MediaControlsLibrary;
using Microsoft.VisualBasic.FileIO;

namespace Media_Manager
{
    public class DiscardElements
    {
        #region Remove Elements
        // Remove Folder
        // ========================================
        // ========================================
        public static bool RemoveFolder(MediaType type, int selectedid)
        {
            //Variables
            MessageBoxResult result;

            //Validate Media Type
            if (type == MediaType.TVShows)
            {
                //Confirm with the user if they would like to remove the folder
                result = CustomMessageBox.ShowYesNo($"Are you sure you would like to remove the folder from the application?\n\nPlease note that this will also remove any folders, tv shows, seasons, and episodes contained within the folder.", "WARNING", "Yes", "No", MessageBoxImage.Warning);
            }
            else
            {
                //Confirm with the user if they would like to remove the folder
                result = CustomMessageBox.ShowYesNo($"Are you sure you would like to remove the folder from the application?\n\nPlease note that this will also remove any folders and items contained within the folder.", "WARNING", "Yes", "No", MessageBoxImage.Warning);
            }

            //Check if the result was yes
            if (result == MessageBoxResult.Yes)
            {
                //Remove Selected Folder
                Database.RemoveFolder(type, selectedid);

                //Return True
                return true;
            }

            //Return False
            return false;
        }


        // Remove TV Show Folder
        // ========================================
        // ========================================
        public static bool RemoveTVShowFolder(RemovalType removaltype, TVShowFolder tvshowfolder)
        {
            //Initialize isRemoval Boolean
            bool isRemoval = false;

            //Check if type is Remove
            if (removaltype == RemovalType.Remove)
            {
                //Confirm with the user if they would like to remove the tv show
                MessageBoxResult result = CustomMessageBox.ShowYesNo($"Are you sure you would like to remove the tv show from the application?\n\nPlease note that this will also remove any seasons and episodes contained within the tv show.", "WARNING", "Yes", "No", MessageBoxImage.Warning);

                //Check if the result was yes
                if (result == MessageBoxResult.Yes)
                {
                    //Set isRemoval to True
                    isRemoval = true;
                }
            }

            //Check if the isRemoval boolean variable is set to true or if the type is delete
            if (isRemoval || removaltype == RemovalType.Delete)
            {
                //Remove Selected TV Show Folder
                Database.RemoveTVShowFolder(tvshowfolder);

                //Return True
                return true;
            }

            //Return False
            return false;
        }


        // Remove Season Folder
        // ========================================
        // ========================================
        public static bool RemoveSeasonFolder(RemovalType removaltype, SeasonFolder seasonfolder, bool iscustomcoverimageused, bool iscustomcoverimageparent)
        {
            //Initialize isRemoval Boolean
            bool isRemoval = false;

            //Check if type is Remove
            if (removaltype == RemovalType.Remove)
            {
                //Confirm with the user if they would like to remove the season
                MessageBoxResult result = CustomMessageBox.ShowYesNo($"Are you sure you would like to remove the season from the application?\n\nPlease note that this will also remove any episodes contained within the season.", "WARNING", "Yes", "No", MessageBoxImage.Warning);

                //Check if the result was yes
                if (result == MessageBoxResult.Yes)
                {
                    //Set isRemoval to True
                    isRemoval = true;
                }
            }

            //Check if the isRemoval boolean variable is set to true or if the type is delete
            if (isRemoval || removaltype == RemovalType.Delete)
            {
                //Remove Selected Season Folder
                Database.RemoveSeasonFolder(seasonfolder, iscustomcoverimageused, iscustomcoverimageparent);

                //Return True
                return true;
            }

            //Return False
            return false;
        }


        // Remove Item
        // ========================================
        // ========================================
        public static bool RemoveItem(MediaType type, int id, RemovalType removaltype, string coverImage = "")
        {
            //Initialize isRemoval Boolean
            bool isRemoval = false;

            //Check if type is Remove
            if (removaltype == RemovalType.Remove)
            {
                //Get Item Type
                string itemtype = GetItemType(type);

                //Confirm with the user if they would like to remove the item
                MessageBoxResult result = CustomMessageBox.ShowYesNo($"Are you sure you would like to remove the {itemtype} from the application?", "WARNING", "Yes", "No", MessageBoxImage.Warning);

                //Check if the result was yes
                if (result == MessageBoxResult.Yes)
                {
                    //Set isRemoval to True
                    isRemoval = true;
                }
            }

            //Check if the isRemoval boolean variable is set to true or if the type is delete
            if (isRemoval || removaltype == RemovalType.Delete)
            {
                //Remove Selected Item
                Database.RemoveItem(type, id, coverImage);

                //Return True
                return true;
            }

            //Return False
            return false;
        }
        #endregion Remove Elements




        #region Delete Elements
        #region Methods
        // Delete Season
        // ========================================
        // ========================================
        public static void DeleteTVShowFolder(out bool isCancel, out bool isDeleted, TVShowFolder tvshowfoler, List<SeasonFolder> seasonfolders, List<Episode> episodes)
        {
            isCancel = false;
            isDeleted = false;

            if (tvshowfoler == null || seasonfolders == null || episodes == null)
            {
                return;
            }

            //Initialize Variables
            bool isoverride = false;

            //Confirm Deletion with User
            MessageBoxResult result = CustomMessageBox.ShowYesNoCancel($"Are you sure you would like to delete the tv show from the computer system?\n\nPlease note for each tv show season, all episodes will be deleted before folder deletion validation can take place which will verify the season folders contents and validate deletion.", "WARNING", "Delete", "Recycle", "Cancel", MessageBoxImage.Warning);

            //Check if result is delete or recycle
            if (result == MessageBoxResult.Yes || result == MessageBoxResult.No)
            {
                //Get Deletion Type
                RecycleOption recycleoption = GetDeletionType(result);

                try
                {
                    //Validate all paths and obtain consent for untracked files before deleting anything.
                    for (int i = 0; i < seasonfolders.Count; i++)
                    {
                        if (!Directory.Exists(seasonfolders[i].FilePath))
                        {
                            return;
                        }

                        List<Episode> seasonEpisodes = episodes.Where(j => j.OwnerId == seasonfolders[i].Id).ToList();
                        if (seasonEpisodes.Any(item => !File.Exists(item.FilePath)))
                        {
                            return;
                        }

                        HashSet<string> trackedPaths = new HashSet<string>(
                            seasonEpisodes.Select(item => Path.GetFullPath(item.FilePath)),
                            StringComparer.OrdinalIgnoreCase);
                        bool containsUntrackedFiles = Directory
                            .EnumerateFiles(seasonfolders[i].FilePath, "*", System.IO.SearchOption.AllDirectories)
                            .Any(path => !trackedPaths.Contains(Path.GetFullPath(path)));

                        if (containsUntrackedFiles && !isoverride)
                        {
                            result = CustomMessageBox.ShowYesNoCancel($"Are you sure you would like to delete season {seasonfolders[i].SeasonNumber}'s folder from the computer system?\n\nPlease note this will also delete files that are not tracked by Media Manager.", "WARNING", "Yes", "Yes to All", "Cancel", MessageBoxImage.Warning);

                            if (result != MessageBoxResult.Yes && result != MessageBoxResult.No)
                            {
                                isCancel = true;
                                return;
                            }

                            if (result == MessageBoxResult.No)
                            {
                                isoverride = true;
                            }
                        }
                    }

                    //Loop through Season Folders
                    for (int i = 0; i < seasonfolders.Count; i++)
                    {
                        //Get Current Looped Season's Episodes
                        List<Episode> episodestoremove = episodes.Where(j => j.OwnerId == seasonfolders[i].Id).ToList();

                        //Loop through and Delete Current Looped Season's Episodes
                        foreach (Episode item in episodestoremove)
                        {
                            if (!DeletePath(recycleoption, item.FilePath, FileType.File))
                            {
                                return;
                            }
                        }

                        //Delete Season Folder
                        if (!DeletePath(recycleoption, seasonfolders[i].FilePath, FileType.Folder))
                        {
                            return;
                        }
                    }

                    isDeleted = true;
                    return;
                }
                catch (Exception exception)
                {
                    CustomMessageBox.ShowOK($"The TV show could not be deleted.\n\n{exception.Message}", "ERROR", "OK", MessageBoxImage.Error);
                    return;
                }
            }
            else if (result == MessageBoxResult.Cancel || result == MessageBoxResult.None)
            {
                //Set isCancel to True
                isCancel = true;

                //Set isDeleted to False
                isDeleted = false;

                //Return Method
                return;
            }

        }


        // Delete Season
        // ========================================
        // ========================================
        public static void DeleteSeasonFolder(out bool isCancel, out bool isDeleted, SeasonFolder selectedSeasonFolder, List<Episode> episodes)
        {
            isCancel = false;
            isDeleted = false;

            if (selectedSeasonFolder == null || episodes == null || !Directory.Exists(selectedSeasonFolder.FilePath))
            {
                return;
            }

            //Initialize Variables
            string yes = "Delete", no = "Recycle";

            //Confirm Deletion with User
            MessageBoxResult result = CustomMessageBox.ShowYesNoCancel($"Are you sure you would like to delete the season from the computer system?\n\nPlease note all episodes will be deleted before folder deletion validation can take place which will verify the season folders contents and validate deletion.", "WARNING", yes, no, "Cancel", MessageBoxImage.Warning);

            //Check if result is delete or recycle
            if (result == MessageBoxResult.Yes || result == MessageBoxResult.No)
            {
                //Get Deletion Type
                RecycleOption recycleoption = GetDeletionType(result);

                try
                {
                    //Validate all paths and obtain consent for untracked files before deleting anything.
                    if (episodes.Any(item => !File.Exists(item.FilePath)))
                    {
                        return;
                    }

                    HashSet<string> trackedPaths = new HashSet<string>(
                        episodes.Select(item => Path.GetFullPath(item.FilePath)),
                        StringComparer.OrdinalIgnoreCase);
                    bool containsUntrackedFiles = Directory
                        .EnumerateFiles(selectedSeasonFolder.FilePath, "*", System.IO.SearchOption.AllDirectories)
                        .Any(path => !trackedPaths.Contains(Path.GetFullPath(path)));

                    if (containsUntrackedFiles)
                    {
                        string type = result == MessageBoxResult.Yes ? yes : no;
                        result = CustomMessageBox.ShowYesNo($"Are you sure you would like to delete season {selectedSeasonFolder.SeasonNumber}'s folder from the computer system?\n\nPlease note this will also delete files that are not tracked by Media Manager.", "WARNING", type, "Cancel", MessageBoxImage.Warning);

                        if (result != MessageBoxResult.Yes)
                        {
                            isCancel = true;
                            return;
                        }
                    }

                    //Loop through and Delete Episodes
                    foreach (Episode item in episodes)
                    {
                        if (!DeletePath(recycleoption, item.FilePath, FileType.File))
                        {
                            return;
                        }
                    }

                    //Delete Season Folder
                    isDeleted = DeletePath(recycleoption, selectedSeasonFolder.FilePath, FileType.Folder);
                    return;
                }
                catch (Exception exception)
                {
                    CustomMessageBox.ShowOK($"The season could not be deleted.\n\n{exception.Message}", "ERROR", "OK", MessageBoxImage.Error);
                    return;
                }
            }
            else if (result == MessageBoxResult.Cancel || result == MessageBoxResult.None)
            {
                //Set isCancel to True
                isCancel = true;

                //Set isDeleted to False
                isDeleted = false;

                //Return Method
                return;
            }

        }


        // Delete Item
        // ========================================
        // ========================================
        public static void DeleteItem(out bool isCancel, out bool isDeleted, MediaType type, FileType filetype, string path)
        {
            isCancel = false;
            isDeleted = false;

            if (!PathExists(path, filetype))
            {
                return;
            }

            //Get Item Type
            string itemtype = GetItemType(type);

            //Confirm Deletion with User
            MessageBoxResult result = CustomMessageBox.ShowYesNoCancel($"Are you sure you would like to delete the {itemtype} from the computer system?", "WARNING", "Delete", "Recycle", "Cancel", MessageBoxImage.Warning);

            //Check if result is delete or recycle
            if (result == MessageBoxResult.Yes || result == MessageBoxResult.No)
            {
                //Get Deletion Type
                RecycleOption recycleoption = GetDeletionType(result);

                try
                {
                    //Delete Item
                    isDeleted = DeletePath(recycleoption, path, filetype);
                    return;
                }
                catch (Exception exception)
                {
                    CustomMessageBox.ShowOK($"The {itemtype} could not be deleted.\n\n{exception.Message}", "ERROR", "OK", MessageBoxImage.Error);
                    return;
                }
            }
            else if (result == MessageBoxResult.Cancel || result == MessageBoxResult.None)
            {
                //Set isCancel to True
                isCancel = true;

                //Set isDeleted to False
                isDeleted = false;

                //Return Method
                return;
            }

        }
        #endregion Methods



        #region Extensions
        // Delete File
        // ========================================
        // ========================================
        private static bool DeletePath(RecycleOption recycleOption, string path, FileType filetype)
        {
            if (!PathExists(path, filetype))
            {
                return false;
            }

            //Check File Type
            if (filetype == FileType.File)
            {
                //Set File's Attributes to Normal
                File.SetAttributes(path, FileAttributes.Normal);

                //Delete File
                FileSystem.DeleteFile(path, UIOption.OnlyErrorDialogs, recycleOption, UICancelOption.ThrowException);

                return !File.Exists(path);
            }
            else if(filetype == FileType.Folder)
            {
                //Create DirectoryInfo Object for Game Directory
                DirectoryInfo directoryInfo = new DirectoryInfo(path);

                //Loop through Files to Set Their Attributes Value to Normal
                foreach (FileInfo file in directoryInfo.EnumerateFiles("*", System.IO.SearchOption.AllDirectories))
                {
                    file.Attributes = FileAttributes.Normal;
                }

                //Set Game Directory's Attributes to Normal
                directoryInfo.Attributes = FileAttributes.Normal;

                //Delete Directory
                FileSystem.DeleteDirectory(path, UIOption.OnlyErrorDialogs, recycleOption, UICancelOption.ThrowException);

                return !Directory.Exists(path);
            }

            return false;
        }

        private static bool PathExists(string path, FileType filetype)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            return filetype == FileType.Folder
                ? Directory.Exists(path)
                : File.Exists(path);
        }


        // Get Deletion Type
        // ========================================
        // ========================================
        private static RecycleOption GetDeletionType(MessageBoxResult result)
        {
            //Check if Result is Set to MessageBoxResult Yes
            if (result == MessageBoxResult.Yes)
            {
                //Return RecycleOption DeletePermanently
                return RecycleOption.DeletePermanently;
            }

            //Return RecycleOption SendToRecycleBin
            return RecycleOption.SendToRecycleBin;
        }
        #endregion Extensions
        #endregion Delete Elements




        // Extensions
        // ========================================
        // ========================================
        // ========================================
        private static string GetItemType(MediaType type)
        {
            //Get Media Type
            string mediatype = type.ToString().TrimEnd('s').ToLower();

            //Get and Return Item Type
            return mediatype == "music" ? "song" : mediatype;
        }
    }
}

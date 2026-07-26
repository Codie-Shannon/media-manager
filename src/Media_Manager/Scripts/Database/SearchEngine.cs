using System.Linq;
using OpenQA.Selenium;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MediaControlsLibrary.Models;
using System.Threading;
using System.Windows;
using System;
using MediaControlsLibrary;
using SeleniumUndetectedChromeDriver;
using System.Diagnostics;

namespace Media_Manager
{
    public class SearchEngine
    {
        #region Variables
        // Chrome Driver
        // =================================================
        // =================================================
        private static UndetectedChromeDriver DefaultDriver;
        private static UndetectedChromeDriver IMDBDriver;


        // Search Thread
        // =================================================
        // =================================================
        private static Thread SearchThread;
        private static int SearchVersion;
        private static readonly SemaphoreSlim SearchGate = new SemaphoreSlim(1, 1);
        #endregion Variables



        #region Operation
        // Start
        // =================================================
        // =================================================
        public static async Task StartAsync(string defaultlink, string imdblink = "")
        {
            //Validate Internet Connection
            if (Internet.Validate())
            {
                //Set Active Window to Topmost
                WindowsAPI.ToggleActiveWindow(true);

                //Create Default Driver
                DefaultDriver = UndetectedChromeDriver.Create(null, null, await new ChromeDriverInstaller().Auto(), null, 0, 0, false, true, true, true);

                //Validate and Create IMDB Driver
                if (!string.IsNullOrEmpty(imdblink)) { IMDBDriver = UndetectedChromeDriver.Create(null, null, await new ChromeDriverInstaller().Auto(), null, 0, 0, false, true, true, true); }

                //Hide Chrome Driver Windows
                WindowsAPI.HideChromeDriverWindows();

                //Unset Active Window Topmost
                WindowsAPI.ToggleActiveWindow(false);

                //Navigate Default Driver to the defaultlink Variable Value
                DefaultDriver.Navigate().GoToUrl(defaultlink);

                //Navigate IMDB Driver to the imdblink Variable Value
                if (!string.IsNullOrEmpty(imdblink)) { IMDBDriver.Navigate().GoToUrl(imdblink); }
            }
        }


        // Abort
        // =================================================
        // =================================================
        public static async Task Abort(optSearchBox searchbox)
        {
            //Show Loading Text
            searchbox.isLoading = true;

            //Invalidate and stop any previous search operation.
            int version = Interlocked.Increment(ref SearchVersion);
            Thread activeThread = SearchThread;
            await Task.Run(() =>
            {
                StopDrivers();

                if (activeThread != null
                    && activeThread != Thread.CurrentThread
                    && activeThread.IsAlive)
                {
                    activeThread.Join(5000);
                }
            });

            //Hide Loading Text
            if (version == Volatile.Read(ref SearchVersion))
            {
                searchbox.isLoading = false;
            }
        }


        // Stop
        // =================================================
        // =================================================
        public static void Stop()
        {
            //Invalidate the current operation before closing its browser drivers.
            Interlocked.Increment(ref SearchVersion);
            StopDrivers();
        }

        private static void StopDrivers()
        {
            UndetectedChromeDriver defaultDriver = DefaultDriver;
            UndetectedChromeDriver imdbDriver = IMDBDriver;
            DefaultDriver = null;
            IMDBDriver = null;

            if (defaultDriver != null)
            {
                try
                {
                    defaultDriver.Quit();
                }
                catch (WebDriverException)
                {
                }
                catch (InvalidOperationException)
                {
                }
                catch (Exception)
                {
                }
            }

            if (imdbDriver != null)
            {
                try
                {
                    imdbDriver.Quit();
                }
                catch (WebDriverException)
                {
                }
                catch (InvalidOperationException)
                {
                }
                catch (Exception)
                {
                }
            }
        }
        #endregion Operation



        #region Search
        // Virtual Entertainment Search
        // ===========================================================
        // ===========================================================
        public static void VirtualEntertainmentSearch(optSearchBox searchbox, string search, MediaType mediatype = MediaType.Movies)
        {
            //Show Loading Text
            searchbox.isLoading = true;
            int version = Interlocked.Increment(ref SearchVersion);

            //Create Search Thread
            Thread thread = new Thread(() =>
            {
                SearchGate.Wait();

                try
                {
                    if (version != Volatile.Read(ref SearchVersion))
                    {
                        return;
                    }

                    List<MovieSearch> results = RetrieveVirtualEntertainmentSearchAsync(
                        search,
                        mediatype).GetAwaiter().GetResult();

                    if (version == Volatile.Read(ref SearchVersion))
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            searchbox.Clear(true);

                            foreach (MovieSearch movie in results)
                            {
                                searchbox.Add(movie.Name, movie.CoverImage, movie.MetacriticLink, movie.IMDBLink);
                            }
                        }));
                    }
                }
                catch (Exception exception)
                {
                    ShowSearchFailure(searchbox, version, exception);
                }
                finally
                {
                    CompleteSearch(searchbox, version);
                    SearchGate.Release();
                }
            });
            thread.IsBackground = true;
            SearchThread = thread;

            //Run Search Thread
            thread.Start();
        }

        private static async Task<List<MovieSearch>> RetrieveVirtualEntertainmentSearchAsync(string search, MediaType mediatype)
        {
            await StartAsync(
                FormatURL(mediatype, search, "https://www.metacritic.com/search/"),
                FormatURL(
                    mediatype,
                    search,
                    "https://www.imdb.com/find/?q=",
                    "&s=tt&ttype=ft&ref_=fn_ft",
                    "&s=tt&ttype=tv&ref_=fn_tv"));

            Task<List<MovieSearch>> metacriticTask = RetrieveVirtualEntertainmentResultsAsync(
                DriverType.Default,
                DefaultDriver,
                "c-pageSiteSearch-results-item",
                mediatype);
            Task<List<MovieSearch>> imdbTask = RetrieveVirtualEntertainmentResultsAsync(
                DriverType.IMDB,
                IMDBDriver,
                "find-result-item",
                mediatype);

            await Task.WhenAll(metacriticTask, imdbTask);
            return CombineVirtualEntertainmentResults(
                metacriticTask.Result,
                imdbTask.Result);
        }

        private static async Task<List<MovieSearch>> RetrieveVirtualEntertainmentResultsAsync(DriverType type, IWebDriver driver, string element_classname, MediaType mediatype)
        {
            //Variables
            List<MovieSearch> results = new List<MovieSearch>();

            if (driver == null)
            {
                return results;
            }

            try
            {
                //Validate Search Items
                if (IsElementPresent(type, By.ClassName(element_classname), null, driver))
                {
                    //Check if the Driver Type is Metacritic
                    if (type == DriverType.Default)
                    {
                        //Get Category Index (Movies = 3, TV Shows = 4)
                        int index = mediatype == MediaType.Movies ? 3 : 4;

                        //Get Metacritic Window
                        Process metacriticwindow = WindowsAPI.RetrieveProcesses("Metacritic").FirstOrDefault();

                        //Set Foreground Window
                        if (metacriticwindow != null)
                        {
                            WindowsAPI.SetWindowFocus(metacriticwindow.MainWindowHandle);
                        }

                        //Get Category Button
                        IWebElement categorybtn = driver.FindElement(By.XPath($"/html/body/div[1]/div/div/div[2]/div[1]/div[1]/div/div/ul/li[{index}]"));

                        //Click Category Button
                        categorybtn.Click();

                        //Wait for Category Button to be Clicked
                        await Task.Delay(500);

                        //Set Foreground Window
                        WindowsAPI.SetWindowFocus();

                        //Wait for Website to Load Category
                        await Task.Delay(1000);
                    }

                    //Get Search Items
                    ReadOnlyCollection<IWebElement> elements = driver.FindElements(By.ClassName(element_classname));

                    //Loop through Search Items
                    foreach (IWebElement element in elements)
                    {
                        try
                        {
                            MovieSearch result = new MovieSearch()
                            {
                                Name = type == DriverType.Default ? GetMetacriticTitle(element) : GetIMDBTitle(element),
                                CoverImage = type == DriverType.Default ? GetMetacriticCover(element) : GetIMDBCover(element),
                                MetacriticLink = type == DriverType.Default ? GetMetacriticLink(element) : string.Empty,
                                IMDBLink = type == DriverType.Default ? string.Empty : GetIMDBLink(element)
                            };

                            if (!string.IsNullOrWhiteSpace(result.Name))
                            {
                                results.Add(result);
                            }
                        }
                        catch (WebDriverException)
                        {
                            //A changed or stale result card should not fail the entire search.
                        }
                    }
                }
            }
            catch (WebDriverException)
            {
                //A provider window may close or navigate while results are being read.
            }
            catch (InvalidOperationException)
            {
                //Treat a disposed driver as an unavailable provider.
            }

            //Return results List
            return results;
        }

        private static List<MovieSearch> CombineVirtualEntertainmentResults(List<MovieSearch> metacriticresults, List<MovieSearch> imdbresults)
        {
            metacriticresults = metacriticresults ?? new List<MovieSearch>();
            imdbresults = imdbresults ?? new List<MovieSearch>();

            //Get Loop Type
            string type = metacriticresults.Count > imdbresults.Count ? "imdb" : "metacritic";

            //Loop through the Shortest Results List
            foreach (MovieSearch item in type == "imdb" ? imdbresults : metacriticresults)
            {
                //Convert Current Looped Virtual Entertainment Item Title to Lowercase
                string title = item.Name.ToLowerInvariant();

                //Validate Current Looped Movie Match
                if (type == "imdb" && metacriticresults.Any(i => i.Name.ToLowerInvariant() == title))
                {
                    //Get Match
                    MovieSearch match = metacriticresults.FirstOrDefault(i => i.Name.ToLowerInvariant() == title);

                    //Assign Match's Metacritic Link Value into the Current Looped Virtual Entertainment Item's Metacritic Link Value
                    item.MetacriticLink = match.MetacriticLink;

                    //Attempt to Get Cover Image
                    item.CoverImage = string.IsNullOrEmpty(item.CoverImage) ? match.CoverImage : item.CoverImage;

                    //Remove Match from Metacritic Results List
                    metacriticresults.Remove(match);
                }
                else if (type == "metacritic" && imdbresults.Any(i => i.Name.ToLowerInvariant() == title))
                {
                    //Get Match
                    MovieSearch match = imdbresults.First(i => i.Name.ToLowerInvariant() == title);

                    //Assign Match's IMDB Link Value into the Current Looped Virtual Entertainment Item's IMDB Link Value
                    item.IMDBLink = match.IMDBLink;

                    //Attempt to Get Cover Image
                    item.CoverImage = string.IsNullOrEmpty(item.CoverImage) ? match.CoverImage : item.CoverImage;

                    //Remove Match from IMDB Results List 
                    imdbresults.Remove(match);
                }
            }

            //Combine Results
            imdbresults.AddRange(metacriticresults);

            //Return Results
            return imdbresults;
        }


        // Game Search
        // ===========================================================
        // ===========================================================
        public static void GameSearch(optSearchBox searchbox, string search)
        {
            //Show Loading Text
            searchbox.isLoading = true;
            int version = Interlocked.Increment(ref SearchVersion);

            //Create Search Thread
            Thread thread = new Thread(() =>
            {
                SearchGate.Wait();

                try
                {
                    if (version != Volatile.Read(ref SearchVersion))
                    {
                        return;
                    }

                    List<GameSearch> result = FindGamesAsync(search).GetAwaiter().GetResult();

                    if (version == Volatile.Read(ref SearchVersion))
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            searchbox.Clear(true);

                            foreach (GameSearch game in result)
                            {
                                searchbox.Add(game.IGDBLink, game.Name, game.CoverImage, game.Type, game.Platforms);
                            }
                        }));
                    }
                }
                catch (Exception exception)
                {
                    ShowSearchFailure(searchbox, version, exception);
                }
                finally
                {
                    CompleteSearch(searchbox, version);
                    SearchGate.Release();
                }
            });
            thread.IsBackground = true;
            SearchThread = thread;

            //Run Search Thread
            thread.Start();
        }

        private static async Task<List<GameSearch>> FindGamesAsync(string search)
        {
            //Variables
            List<GameSearch> results = new List<GameSearch>();

            //Start Search
            await StartAsync(FormatURL(MediaType.Games, search));

            if (DefaultDriver == null)
            {
                return results;
            }

            //Get Search Results
            ReadOnlyCollection<IWebElement> elements;
            try
            {
                elements = DefaultDriver.FindElements(By.ClassName("media"));
            }
            catch (WebDriverException)
            {
                return results;
            }

            //Loop through Results
            foreach (IWebElement element in elements)
            {
                try
                {
                    //Add Search Item to results List
                    results.Add(new GameSearch()
                    {
                        IGDBLink = GetGameLink(element),
                        Name = GetGameTitle(element),
                        CoverImage = GetGameCover(element),
                        Type = GetGameType(element),
                        Platforms = GetGamePlatforms(element)
                    });
                }
                catch (WebDriverException)
                {
                    //Skip stale result cards.
                }
            }

            //Return Results KeyValuePair
            return results;
        }

        private static void ShowSearchFailure(optSearchBox searchbox, int version, Exception exception)
        {
            if (version != Volatile.Read(ref SearchVersion))
            {
                return;
            }

            StopDrivers();

            try
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    searchbox.Clear(true);
                    CustomMessageBox.ShowOK(
                        $"The metadata search could not be completed.\n\n{exception.Message}",
                        "ERROR",
                        "OK",
                        MessageBoxImage.Error);
                }));
            }
            catch (Exception)
            {
                //The application may already be shutting down.
            }
        }

        private static void CompleteSearch(optSearchBox searchbox, int version)
        {
            StopDrivers();

            if (version == Volatile.Read(ref SearchVersion))
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        searchbox.isLoading = false;
                    }));
                }
                catch (Exception)
                {
                    //The application may already be shutting down.
                }
            }

            if (ReferenceEquals(SearchThread, Thread.CurrentThread))
            {
                SearchThread = null;
            }
        }
        #endregion Search



        #region Extensions
        // Format URL
        // =================================================
        // =================================================
        public static string FormatURL(MediaType type, string search, string start = "", string end = "", string altend = "")
        {
            //Validate Type
            if (type == MediaType.Games)
            {
                //Format Search Text
                search = search.Replace(" ", "+");

                //Return Formatted URL
                return "https://www.igdb.com/search?type=1&q=" + search;
            }
            else if (type == MediaType.Movies || type == MediaType.TVShows)
            {
                //Format Search Text
                search = search.Replace(" ", "%20");

                //Return Formatted URL
                return type == MediaType.Movies ? $"{start}{search}{end}" : $"{start}{search}{altend}";
            }

            //Return Empty String
            return string.Empty;
        }


        // Is Element Present
        // =================================================
        // =================================================
        private static bool IsElementPresent(DriverType type, By by, IWebElement parent = null, IWebDriver driver = null)
        {
            //Run Try Statement
            try
            {
                IWebDriver activeDriver = driver
                    ?? (type == DriverType.Default ? DefaultDriver : IMDBDriver);

                if (parent == null && activeDriver == null)
                {
                    return false;
                }

                //Find Element
                _ = parent != null
                    ? parent.FindElement(by)
                    : activeDriver.FindElement(by);

                //Return Valid
                return true;
            }
            catch (NoSuchElementException)
            {
                //Return Invalid
                return false;
            }
            catch (WebDriverException)
            {
                //A browser window may have closed or navigated.
                return false;
            }
            catch (InvalidOperationException)
            {
                //A disposed driver is equivalent to a missing element.
                return false;
            }
        }
        #endregion Extensions



        #region Virtual Entertainment
        #region Metacritic
        // Metacritic Title
        // ===========================================================
        // ===========================================================
        private static string GetMetacriticTitle(IWebElement parent) { return parent.FindElement(By.TagName("p")).Text; }


        // Metacritic Cover
        // ===========================================================
        // ===========================================================
        private static string GetMetacriticCover(IWebElement parent) { return IsElementPresent(DriverType.Null, By.TagName("img"), parent) ? parent.FindElement(By.TagName("img")).GetAttribute("src") : string.Empty; }


        // Metacritic Link
        // ===========================================================
        // ===========================================================
        private static string GetMetacriticLink(IWebElement parent) { return parent.GetAttribute("href"); }
        #endregion Metacritic



        #region IMDB
        // IMDB Title
        // ===========================================================
        // ===========================================================
        private static string GetIMDBTitle(IWebElement parent) { return GetGenericIMDBDetail(parent); }


        // IMDB Cover
        // ===========================================================
        // ===========================================================
        private static string GetIMDBCover(IWebElement parent) { return GetGenericIMDBDetail(parent, "src", "ipc-image"); }


        // IMDB Link
        // ===========================================================
        // ===========================================================
        private static string GetIMDBLink(IWebElement parent) { return GetGenericIMDBDetail(parent, "href"); }


        // Generic IMDB Detail
        // ===========================================================
        // ===========================================================
        private static string GetGenericIMDBDetail(IWebElement parent, string attribute = "", string classname = "ipc-metadata-list-summary-item__t")
        {
            //Validate Detail IWebElement
            if (IsElementPresent(DriverType.Null, By.ClassName(classname), parent))
            {
                //Get Detail IWebElement
                IWebElement element = parent.FindElement(By.ClassName(classname));

                //Get and Return Detail Value
                return string.IsNullOrEmpty(attribute) ? element.Text : element.GetAttribute(attribute);
            }

            //Return Empty String
            return string.Empty;
        }
        #endregion IMDB
        #endregion Virtual Entertainment



        #region Game
        // Game Link
        // =================================================
        // =================================================
        private static string GetGameLink(IWebElement parent) { return GetGenericGameDetail(parent, "media-body", "a", "href"); }


        // Title
        // =================================================
        // =================================================
        private static string GetGameTitle(IWebElement parent) { return GetGenericGameDetail(parent, "media-body", "a"); }


        // Cover
        // =================================================
        // =================================================
        private static string GetGameCover(IWebElement parent) { return GetGenericGameDetail(parent, "game_cover", "img", "src"); }


        // Type
        // =================================================
        // =================================================
        private static string GetGameType(IWebElement parent)
        {
            //Validate Game Type
            if (IsElementPresent(DriverType.Null, By.ClassName("game-type"), parent))
            {
                //Get and Return Game Type
                return parent.FindElement(By.ClassName("game-type")).Text;
            }

            //Return Empty String
            return string.Empty;
        }


        // Platforms
        // =================================================
        // =================================================
        private static List<string> GetGamePlatforms(IWebElement parent)
        {
            //Variables
            List<string> platforms = new List<string>();

            //Get Media Body Parent
            parent = parent.FindElement(By.ClassName("media-body"));

            //Validate Platform Parent
            if(IsElementPresent(DriverType.Null, By.TagName("div"), parent))
            {
                //Get Platform Parent
                parent = parent.FindElement(By.TagName("div"));
                
                //Validate Platforms
                if(IsElementPresent(DriverType.Null, By.TagName("a"), parent) && parent.FindElement(By.TagName("a")).GetAttribute("href").Contains("platform"))  //divs.Any(i => IsElementPresent(DriverType.Null, By.TagName("a"), i)))
                {
                    //Get Platforms
                    ReadOnlyCollection<IWebElement> elements = parent.FindElements(By.TagName("a"));

                    //Loop through Platform Elements
                    foreach (IWebElement element in elements)
                    {
                        //Add Current Looped Platform Value to List
                        platforms.Add(element.Text);
                    }
                }
            }

            //Return platforms List
            return platforms;
        }


        // Generic Game Detail
        // =================================================
        // =================================================
        private static string GetGenericGameDetail(IWebElement parent, string classname, string tagname, string attribute = "")
        {
            //Get Media Body Parent
            parent = parent.FindElement(By.ClassName(classname));

            //Validate Detail IWebElement
            if (IsElementPresent(DriverType.Null, By.TagName(tagname), parent))
            {
                //Get Detail IWebElement
                IWebElement element = parent.FindElement(By.TagName(tagname));

                //Get and Return Detail
                return string.IsNullOrEmpty(attribute) ? element.Text : element.GetAttribute(attribute);
            }

            //Return Empty String
            return string.Empty;
        }
        #endregion Game
    }
}

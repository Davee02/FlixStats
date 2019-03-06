using HtmlAgilityPack;
using NetflixStatizier.Stats.Model;
using Newtonsoft.Json;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NetflixStatizier.Stats.Exceptions;
using Cookie = OpenQA.Selenium.Cookie;

namespace NetflixStatizier.Stats
{
    public class NetflixViewingHistoryLoader
    {
        private const string NETFLIX_VIEWINGACTIVITY_URL = "https://www.netflix.com/viewingactivity";
        private const string NETFLIX_LOGINPAGE_URL = "https://www.netflix.com/login";
        private const string MAILADRESS_TEXTBOX_ID = "#id_userLoginId";
        private const string PASSWORD_TEXTBOX_ID = "#id_password";
        private const string LOGIN_BUTTON_ID = "button[type='submit']";

        public string NetflixPassword { get; set; }
        public string NetflixEmail { get; set; }

        public NetflixViewingHistoryLoader(string netflixEmail, string netflixPassword)
        {
            if(string.IsNullOrEmpty(netflixEmail))
                throw new ArgumentException("The netflix profile email must not be empty", nameof(netflixEmail));
            if (string.IsNullOrEmpty(netflixPassword))
                throw new ArgumentException("The netflix profile password must not be empty", nameof(netflixEmail));

            NetflixEmail = netflixEmail;
            NetflixPassword = netflixPassword;
        }

        public async Task<IEnumerable<NetflixPlayback>> GetNetflixViewingHistory(string netflixProfileName, IWebDriver driver)
        {
            var cookies = LogInToNetflixAndGetCookies(netflixProfileName, driver);

            var historyJson = await GetViewingHistoryJson();
            var apiBaseUrl = GetViewingActivityBaseUrl(historyJson);

            var viewedElements = await GetAllViewedElements(apiBaseUrl, cookies);

            return GetNetflixPlaybacksFromViewingActivity(viewedElements);
        }

        private IEnumerable<Cookie> LogInToNetflixAndGetCookies(string netflixProfileName, IWebDriver driver)
        {
            driver.Navigate().GoToUrl(NETFLIX_LOGINPAGE_URL);

            var mailAdressTextBox = driver.FindElement(By.CssSelector(MAILADRESS_TEXTBOX_ID));
            var passwordAdressTextBox = driver.FindElement(By.CssSelector(PASSWORD_TEXTBOX_ID));
            var logInButton = driver.FindElement(By.CssSelector(LOGIN_BUTTON_ID));

            mailAdressTextBox.SendKeys(NetflixEmail);
            passwordAdressTextBox.SendKeys(NetflixPassword);
            logInButton.Click();

            SearchForErrorBoxesAndThrowIfNecessary(driver);

            var profileButton = driver.FindElements(By.CssSelector("span[class='profile-name']"))
                .FirstOrDefault(x => string.Equals(x.Text, netflixProfileName, StringComparison.InvariantCultureIgnoreCase));
            if (profileButton == null)
                throw new ArgumentException($"There is no profile with the name {netflixProfileName} in this account",
                    nameof(netflixProfileName));

            profileButton.Click();

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            var test = driver.FindElement(By.CssSelector("h2[class='rowHeader']"));


            return driver.Manage().Cookies.AllCookies;
        }

        private static void SearchForErrorBoxesAndThrowIfNecessary(ISearchContext webDriver)
        {
            var errorBox = webDriver.FindElements(By.CssSelector("div[data-uia='error-message-container']"));

            if (!errorBox.Any())
                return;

            var errorMessage = errorBox[0].Text;
            throw new NetflixLoginException(errorMessage);

        }

        private static async Task<string> GetViewingHistoryJson()
        {
            var htmlWeb = new HtmlWeb();
            var htmlDocument = await htmlWeb.LoadFromWebAsync(NETFLIX_VIEWINGACTIVITY_URL);
            var jsonNodeText = htmlDocument.DocumentNode.SelectSingleNode("/html/body/div[2]/script[1]").InnerText;

            jsonNodeText = jsonNodeText
                .Replace("window.netflix = window.netflix || {} ;         netflix.reactContext = ", string.Empty)
                .Replace(@"\", @"\\")
                .Replace(@"\""", @"\\"""); // '\"' → '\\"'

            return jsonNodeText.Remove(jsonNodeText.Length - 1);
        }

        private string GetViewingActivityBaseUrl(string viewingActivityPageJson)
        {
            if (string.IsNullOrEmpty(viewingActivityPageJson) || !viewingActivityPageJson.IsValidJson())
                throw new ArgumentException("The provided json is not valid.", nameof(viewingActivityPageJson));

            dynamic parsedJson = JsonConvert.DeserializeObject(viewingActivityPageJson);
            return $"{parsedJson.models.serverDefs.data.SHAKTI_API_ROOT}/{parsedJson.models.serverDefs.data.BUILD_IDENTIFIER}/viewingactivity"
                    .Replace(@"\x2F", "/");
        }

        private static async Task<IEnumerable<NetflixViewingHistoryPart>> GetAllViewedElements(string apiBaseUrl, IEnumerable<Cookie> cookies)
        {
            var counter = 0;
            var viewingHistory = new List<NetflixViewingHistoryPart>();

            using (var client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.Cookie, Utilities.GetKeyValueStringOutOfCookieCollection(cookies));

                NetflixViewingHistoryPart currentViewingHistoryPartElement;
                do
                {
                    var jsonString = await client.DownloadStringTaskAsync($"{apiBaseUrl}?pg={counter}&pgsize=1000");
                    currentViewingHistoryPartElement = JsonConvert.DeserializeObject<NetflixViewingHistoryPart>(jsonString);
                    counter++;

                    viewingHistory.Add(currentViewingHistoryPartElement);
                } while (currentViewingHistoryPartElement.ViewedItems.Count > 0);
            }

            return viewingHistory;
        }

        private IEnumerable<NetflixPlayback> GetNetflixPlaybacksFromViewingActivity(
            IEnumerable<NetflixViewingHistoryPart> history)
        {
            foreach (var netflixViewingHistoryPart in history)
            {
                foreach (var netflixViewedItem in netflixViewingHistoryPart.ViewedItems)
                {
                    yield return new NetflixPlayback
                    {
                        PlaybackDateTime = netflixViewedItem.PlaybackDateTime,
                        PlaybackCountry = Utilities.GetRegionInfoFromTwoLetterIsoName(netflixViewedItem.CountryCode),
                        PlaybackDevice = netflixViewedItem.DeviceType,
                        PlaybackDuration = netflixViewedItem.PlaybackBookmark,
                        Sort = netflixViewedItem.Index,
                        Episode = new NetflixEpisode
                        {
                            Duration = netflixViewedItem.Duration,
                            SeasonDescriptor = netflixViewedItem.SeasonDescriptor,
                            Title = netflixViewedItem.EpisodeTitle,
                            EpisodeType = GetEpisodeTypeFromViewedItem(netflixViewedItem),
                            Serie = new NetflixSerie
                            {
                                Title = netflixViewedItem.SeriesTitle,
                                IdentificationNumber = netflixViewedItem.SeriesId
                            }
                        }
                    };
                }
            }
        }

        private static Enums.EpisodeType GetEpisodeTypeFromViewedItem(NetflixViewedItem item)
        {
            return item.SeriesId == 0
                ? Enums.EpisodeType.Movie
                : Enums.EpisodeType.RegularEpisode;
        }
    }
}

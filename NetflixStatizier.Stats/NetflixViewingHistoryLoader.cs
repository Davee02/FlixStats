using HtmlAgilityPack;
using NetflixStatizier.Stats.Model;
using Newtonsoft.Json;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Cookie = OpenQA.Selenium.Cookie;

namespace NetflixStatizier.Stats
{
    public class NetflixViewingHistoryLoader
    {
        private const string NETFLIX_VIEWINGACTIVITY_URL = "https://www.netflix.com/viewingactivity";
        private const string NETFLIX_LOGINPAGE_URL = "https://www.netflix.com/login";

        public string NetflixPassword { get; set; }
        public string NetflixEmail { get; set; }

        public NetflixViewingHistoryLoader(string netflixEmail, string netflixPassword)
        {
            NetflixEmail = netflixEmail;
            NetflixPassword = netflixPassword;
        }

        public async Task<ICollection<NetflixViewingHistoryPart>> GetNetflixViewingHistory(string netflixProfileName, IWebDriver driver)
        {
            var cookies = LogInToNetflixAndGetCookies(netflixProfileName, driver);

            var historyJson = await GetViewingHistoryJson();
            var apiBaseUrl = GetViewingActivityBaseUrl(historyJson);

            return await GetAllViewedElements(apiBaseUrl, cookies);
        }

        private ICollection<Cookie> LogInToNetflixAndGetCookies(string netflixProfileName, IWebDriver driver)
        {
            driver.Navigate().GoToUrl(NETFLIX_LOGINPAGE_URL);

            var mailAdressTextBox = driver.FindElement(By.CssSelector("#id_userLoginId"));
            var passwordAdressTextBox = driver.FindElement(By.CssSelector("#id_password"));
            var logInButton = driver.FindElement(By.CssSelector("button[type='submit']"));

            mailAdressTextBox.SendKeys(NetflixEmail);
            passwordAdressTextBox.SendKeys(NetflixPassword);
            logInButton.Click();

            var profileButton = driver.FindElements(By.CssSelector("span[class='profile-name']"))
                .FirstOrDefault(x => string.Equals(x.Text, netflixProfileName, StringComparison.InvariantCultureIgnoreCase));
            if (profileButton == null)
                throw new ArgumentException($"There is no profile with the name {netflixProfileName} in this account",
                    nameof(netflixProfileName));
            var text = profileButton.Text;
            profileButton.Click();

            return driver.Manage().Cookies.AllCookies;
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

        private static async Task<ICollection<NetflixViewingHistoryPart>> GetAllViewedElements(string apiBaseUrl, ICollection<Cookie> cookies)
        {
            var counter = 0;
            var viewingHistory = new List<NetflixViewingHistoryPart>();

            using (var client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.Cookie, Utilities.GetKeyValueStringOutOfCookieCollection(cookies));

                NetflixViewingHistoryPart currentViewingHistoryPartElement;
                do
                {
                    var jsonString = await client.DownloadStringTaskAsync($"{apiBaseUrl}?pg={counter}");
                    currentViewingHistoryPartElement = JsonConvert.DeserializeObject<NetflixViewingHistoryPart>(jsonString);
                     counter++;

                    viewingHistory.Add(currentViewingHistoryPartElement);
                } while (currentViewingHistoryPartElement.ViewedItems.Count > 0);
            }

            return viewingHistory;
        }
    }
}

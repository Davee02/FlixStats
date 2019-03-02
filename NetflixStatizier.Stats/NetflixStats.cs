﻿using HtmlAgilityPack;
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
    public class NetflixStats
    {
        private const string NETFLIX_VIEWINGACTIVITY_URL = "https://www.netflix.com/viewingactivity";
        private const string NETFLIX_LOGINPAGE_URL = "https://www.netflix.com/login";

        public string NetflixPassword { get; set; }
        public string NetflixEmail { get; set; }

        public NetflixStats(string netflixEmail, string netflixPassword)
        {
            NetflixEmail = netflixEmail;
            NetflixPassword = netflixPassword;
        }

        public async Task<ICollection<NetflixViewingHistory>> GetAllNetflixPlays(string netflixProfileName, IWebDriver driver)
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
            if(profileButton == null)
                throw new ArgumentException($"There is no profile with the name {netflixProfileName} in this account",
                    nameof(netflixProfileName));

            profileButton.Click();

            var cookies = driver.Manage().Cookies.AllCookies
                //.Where(x => string.Equals(x.Name, "NetflixId") || string.Equals(x.Name, "SecureNetflixId"))
                .ToList();

            var historyJson = await GetViewingHistoryJson();
            var apiBaseUrl = GetViewingActivityBaseUrl(historyJson);

            return await GetAllViewedElements(apiBaseUrl, cookies);
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

        private static async Task<ICollection<NetflixViewingHistory>> GetAllViewedElements(string apiBaseUrl, ICollection<Cookie> cookies)
        {
            var counter = 0;
            var viewingHistory = new List<NetflixViewingHistory>();

            using (var client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.Cookie, Utilities.GetKeyValueStringOutOfCookieCollection(cookies));

                NetflixViewingHistory currentViewingHistoryElement;
                do
                {
                    var jsonString = await client.DownloadStringTaskAsync($"{apiBaseUrl}?pg={counter}");
                    currentViewingHistoryElement = JsonConvert.DeserializeObject<NetflixViewingHistory>(jsonString);
                    counter++;

                    viewingHistory.Add(currentViewingHistoryElement);
                } while (currentViewingHistoryElement.ViewedItems.Count > 0);
            }

            return viewingHistory;
        }
    }
}

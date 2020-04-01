using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using FlixStats.Stats.Exceptions;
using FlixStats.Stats.Model;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using Newtonsoft.Json;

namespace FlixStats.Stats
{
    public class NetflixViewingHistoryLoader
    {
        private const string ERROR_BOX_SELECTOR = "div.ui-message-container";
        private const string PROFILE_BUTTON_SELECTOR = "a.profile-link";

        private readonly NetflixProfile _netflixProfile;
        private readonly HttpClient _httpClient;

        public NetflixViewingHistoryLoader(NetflixProfile profile, HttpMessageHandler messageHandler = null)
            : this(messageHandler)
        {
            if (string.IsNullOrEmpty(profile.AccountEmail))
                throw new ArgumentException("The netflix account email must not be empty", nameof(profile.AccountEmail));
            if (string.IsNullOrEmpty(profile.AccountEmail))
                throw new ArgumentException("The netflix account password must not be empty", nameof(profile.AccountPassword));
            if (string.IsNullOrEmpty(profile.ProfileName))
                throw new ArgumentException("The netflix profile-name must not be empty", nameof(profile.ProfileName));

            _netflixProfile = profile;
        }

        public NetflixViewingHistoryLoader(HttpMessageHandler messageHandler = null)
        {
            _httpClient = new HttpClient(messageHandler ?? new HttpClientHandler());
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Safari/537.36");
            _httpClient.BaseAddress = new Uri("https://www.netflix.com");
        }

        public async Task<IEnumerable<NetflixViewedItem>> LoadNetflixViewedItemsAsync()
        {
            if (_netflixProfile != null)
            {
                await LogInToNetflixAndSetCookies();
            }

            var historyJson = await GetViewingHistoryJsonAsync();
            var apiBaseUrl = GetViewingActivityBaseUrl(historyJson);

            var viewedElements = await GetAllViewedElementsAsync(apiBaseUrl);

            return viewedElements.SelectMany(x => x.ViewedItems);
        }

        private async Task LogInToNetflixAndSetCookies()
        {
            var authToken = await GetAuthToken();
            var profileSelectionHtmlDocument = await GetProfileSelectionHtmlDocument(authToken);

            SearchForErrorBoxesAndThrowIfNecessary(profileSelectionHtmlDocument);

            var profileButton = profileSelectionHtmlDocument.QuerySelectorAll(PROFILE_BUTTON_SELECTOR)
                .FirstOrDefault(x => string.Equals(HttpUtility.HtmlDecode(x.InnerText.Trim()), _netflixProfile.ProfileName, StringComparison.OrdinalIgnoreCase));

            if (profileButton == null)
                throw new NetflixProfileNotFoundException($"No profile with the name '{_netflixProfile.ProfileName}' exists in this account")
                {
                    ProfileName = _netflixProfile.ProfileName
                };

            await _httpClient.GetStringAsync($"{profileButton.Attributes["href"].Value}");
        }

        private async Task<string> GetAuthToken()
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(await _httpClient.GetStringAsync("login"));

            return htmlDocument
                .QuerySelector("form.login-form > input[name='authURL']")
                ?.GetAttributeValue("value", "");
        }

        private async Task<HtmlDocument> GetProfileSelectionHtmlDocument(string authToken)
        {
            var formContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "userLoginId", _netflixProfile.AccountEmail },
                { "password", _netflixProfile.AccountPassword },
                { "authURL", authToken },
                { "rememberMe", "false" },
                { "flow", "websiteSignUp" },
                { "mode", "login" },
                { "action", "loginAction" },
                { "withFields", "rememberMe,nextPage,userLoginId,password" }
            });

            var response = await _httpClient.PostAsync("login", formContent);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(await response.Content.ReadAsStringAsync());

            return htmlDocument;
        }

        private static void SearchForErrorBoxesAndThrowIfNecessary(HtmlDocument htmDocument)
        {
            var errorBox = htmDocument.QuerySelectorAll(ERROR_BOX_SELECTOR);

            if (!errorBox.Any())
                return;

            string errorMessage;
            if (errorBox.Count > 1 && errorBox[0].InnerText.Contains("JavaScript", StringComparison.OrdinalIgnoreCase))
                errorMessage = errorBox[1].InnerText;
            else
                errorMessage = errorBox[0].InnerText;

            throw new NetflixLoginException(errorMessage);
        }

        private async Task<string> GetViewingHistoryJsonAsync()
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(await _httpClient.GetStringAsync("viewingactivity"));
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
            return $"api/shakti/{parsedJson.models.serverDefs.data.BUILD_IDENTIFIER}/viewingactivity";
        }

        private async Task<IEnumerable<NetflixViewingHistoryPart>> GetAllViewedElementsAsync(string apiBaseUrl)
        {
            var counter = 0;
            var viewingHistory = new List<NetflixViewingHistoryPart>();

            NetflixViewingHistoryPart currentViewingHistoryPartElement;
            do
            {
                var jsonString = await _httpClient.GetStringAsync($"{apiBaseUrl}?pg={counter}&pgsize=500");
                currentViewingHistoryPartElement = JsonConvert.DeserializeObject<NetflixViewingHistoryPart>(jsonString);
                counter++;

                viewingHistory.Add(currentViewingHistoryPartElement);
            } while (currentViewingHistoryPartElement.ViewedItems.Count > 0);

            return viewingHistory;
        }

        public static IEnumerable<NetflixPlayback> GetNetflixPlaybacksFromViewingActivity(
            IEnumerable<NetflixViewedItem> history)
        {
            foreach (var netflixViewedItem in history)
            {
                yield return new NetflixPlayback
                {
                    PlaybackDateTime = netflixViewedItem.PlaybackDateTime,
                    PlaybackCountry = netflixViewedItem.CountryCode,
                    PlaybackDevice = netflixViewedItem.DeviceType,
                    PlaybackDuration = netflixViewedItem.PlaybackBookmark,
                    Sort = netflixViewedItem.Index,
                    Episode = new NetflixEpisode
                    {
                        Duration = netflixViewedItem.Duration,
                        SeasonDescriptor = netflixViewedItem.SeasonDescriptor,
                        Title = netflixViewedItem.EpisodeTitle ?? netflixViewedItem.VideoTitle,
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

        private static Enums.EpisodeType GetEpisodeTypeFromViewedItem(NetflixViewedItem item)
        {
            return item.SeriesId == 0
                ? Enums.EpisodeType.Movie
                : Enums.EpisodeType.RegularEpisode;
        }
    }
}

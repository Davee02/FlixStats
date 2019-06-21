using HtmlAgilityPack;
using NetflixStatizier.Stats.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack.CssSelectors.NetCore;
using NetflixStatizier.Stats.Exceptions;

namespace NetflixStatizier.Stats
{
    public class NetflixViewingHistoryLoader
    {
        private const string ERROR_BOX_SELECTOR = "div[data-uia='error-message-container']";
        private const string PROFILE_BUTTON_SELECTOR = "a.profile-link";

        private readonly NetflixProfile _netflixProfile;
        private readonly HttpClient _httpClient;
        private readonly Uri _netflixViewingactivityUrl = new Uri("https://www.netflix.com/viewingactivity", UriKind.Absolute);
        private readonly Uri _netflixLoginpageUrl = new Uri("https://www.netflix.com/login", UriKind.Absolute);
        private readonly string _netflixBaseUrl = "https://www.netflix.com";

        public NetflixViewingHistoryLoader(NetflixProfile profile)
        {
            if (string.IsNullOrEmpty(profile.AccountEmail))
                throw new ArgumentException("The netflix account email must not be empty", nameof(profile.AccountEmail));
            if (string.IsNullOrEmpty(profile.AccountEmail))
                throw new ArgumentException("The netflix account password must not be empty", nameof(profile.AccountPassword));
            if (string.IsNullOrEmpty(profile.ProfileName))
                throw new ArgumentException("The netflix profile-name must not be empty", nameof(profile.ProfileName));

            _netflixProfile = profile;

            _httpClient = new HttpClient(new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = new CookieContainer()
            });
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("TWON");
        }


        public async Task<IEnumerable<NetflixViewedItem>> LoadNetflixViewedItemsAsync()
        {
            await LogInToNetflixAndSetCookies();

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
                .FirstOrDefault(x => string.Equals( HttpUtility.HtmlDecode(x.InnerText.Trim()), _netflixProfile.ProfileName, StringComparison.OrdinalIgnoreCase));

            if (profileButton == null)
                throw new NetflixProfileNotFoundException($"There is no profile with the name {_netflixProfile.ProfileName} in this account")
                {
                    ProfileName = _netflixProfile.ProfileName
                };

            await _httpClient.GetStringAsync($"{_netflixBaseUrl}{profileButton.Attributes["href"].Value}");
        }

        private async Task<string> GetAuthToken()
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(await _httpClient.GetStringAsync(_netflixLoginpageUrl));

            return htmlDocument
                .QuerySelector("form.login-form > input[name='authURL']")
                ?.GetAttributeValue("value", "");
        }

        private async Task<HtmlDocument> GetProfileSelectionHtmlDocument(string authToken)
        {
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("userLoginId", _netflixProfile.AccountEmail),
                new KeyValuePair<string, string>("password", _netflixProfile.AccountPassword),
                new KeyValuePair<string, string>("authURL", authToken),
                new KeyValuePair<string, string>("rememberMe", "true"),
                new KeyValuePair<string, string>("flow", "websiteSignUp"),
                new KeyValuePair<string, string>("mode", "login"),
                new KeyValuePair<string, string>("action", "loginAction"),
                new KeyValuePair<string, string>("withFields", "rememberMe,nextPage,userLoginId,password"),
            });

            var response = await _httpClient.PostAsync(_netflixLoginpageUrl, formContent);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(await response.Content.ReadAsStringAsync());

            return htmlDocument;
        }

        private static void SearchForErrorBoxesAndThrowIfNecessary(HtmlDocument htmDocument)
        {
            var errorBox = htmDocument.QuerySelectorAll(ERROR_BOX_SELECTOR);

            if (!errorBox.Any())
                return;

            var errorMessage = errorBox[0].InnerText;
            throw new NetflixLoginException(errorMessage);
        }

        private async Task<string> GetViewingHistoryJsonAsync()
        {
            var htmlWeb = new HtmlWeb();
            var htmlDocument = await htmlWeb.LoadFromWebAsync(_netflixViewingactivityUrl.ToString());
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
            return $"{_netflixBaseUrl}/api/shakti/{parsedJson.models.serverDefs.data.BUILD_IDENTIFIER}/viewingactivity"
                    .Replace(@"\x2F", "/");
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

        private static Enums.EpisodeType GetEpisodeTypeFromViewedItem(NetflixViewedItem item)
        {
            return item.SeriesId == 0
                ? Enums.EpisodeType.Movie
                : Enums.EpisodeType.RegularEpisode;
        }
    }
}

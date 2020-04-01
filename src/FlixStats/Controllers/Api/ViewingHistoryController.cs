using DaHo.Library.Utilities;
using FlixStats.Models.EntityFrameworkModels;
using FlixStats.Stats;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlixStats.Controllers.Api
{
    [ApiController]
    [Route("/api/viewinghistory")]
    public class ViewingHistoryController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NetflixViewedItem>>> GetViewingHistory(string cookies)
        {
            var cookieContainer = new CookieContainer();
            var httpMessageHandler = new HttpClientHandler { CookieContainer = cookieContainer };

            foreach (var cookieString in cookies.RemoveWhitespaces().Split(';'))
            {
                var splittedCookie = cookieString.Split('=');
                var cookie = new Cookie
                {
                    Name = splittedCookie[0],
                    Value = splittedCookie[1],
                    Path = "/",
                    Expires = DateTime.Now.AddDays(1),
                    Domain = ".netflix.com"
                };
                cookieContainer.Add(cookie);
            }

            var historyLoader = new NetflixViewingHistoryLoader(httpMessageHandler);
            var viewedItems = (await historyLoader.LoadNetflixViewedItemsAsync()).ToList();

            return Ok(viewedItems);
        }
    }
}

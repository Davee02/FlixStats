using System;
using Microsoft.AspNetCore.Mvc;
using NetflixStatizier.Stats;
using NetflixStatizier.Stats.Model;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Threading.Tasks;
using NetflixStatizier.Stats.Exceptions;

namespace NetflixStatizier.NetflixApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ViewingHistoryController : ControllerBase
    {
        private readonly IWebDriver _webDriver;

        public ViewingHistoryController(IWebDriver webDriver)
        {
            _webDriver = webDriver;
        }

        // GET api/viewingHistory
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NetflixViewedItem>>> Get(NetflixProfile profile)
        {
            if (string.IsNullOrEmpty(profile.AccountEmail))
                return BadRequest("The netflix account email must not be empty");
            if (string.IsNullOrEmpty(profile.AccountPassword))
                return BadRequest("The netflix account password must not be empty");
            if (string.IsNullOrEmpty(profile.ProfileName))
                return BadRequest("The netflix profile-name must not be empty");

            try
            {
                var historyLoader = new NetflixViewingHistoryLoader(profile);
                return Ok(await historyLoader.LoadNetflixViewedItemsAsync(_webDriver));
            }
            catch (NetflixLoginException e)
            {
                return StatusCode(500, $"An error occured while logging in to Netflix: {e.Message}");
            }
            catch (Exception e)
            {
                return StatusCode(500, $"An error occured while processing the request: {e.Message}");
            }
        }
    }
}

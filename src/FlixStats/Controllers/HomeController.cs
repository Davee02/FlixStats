using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FlixStats.Data.Repositories.Abstractions;
using FlixStats.Models.InputModels;
using FlixStats.Models.ViewModels;
using FlixStats.Stats;
using FlixStats.Stats.Exceptions;
using FlixStats.Stats.Model;
using Microsoft.AspNetCore.Mvc;

namespace FlixStats.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMapper _mapper;
        private readonly INetflixViewedItemRepository _netflixViewedItemRepository;

        public HomeController(
            INetflixViewedItemRepository netflixViewedItemRepository,
            IMapper mapper)
        {
            _netflixViewedItemRepository = netflixViewedItemRepository;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Load(NetflixAccountInputModel model)
        {
            if (!ModelState.IsValid)
                return View("../Home/Index", model);

            var historyLoader = new NetflixViewingHistoryLoader(new NetflixProfile
            {
                AccountEmail = model.NetflixEmail,
                AccountPassword = model.NetflixPassword,
                ProfileName = model.NetflixProfileName
            });

            List<NetflixViewedItem> viewedItems;
            try
            {
                viewedItems = (await historyLoader.LoadNetflixViewedItemsAsync()).ToList();
            }
            catch (NetflixProfileNotFoundException e)
            {
                ModelState.AddModelError("NetflixProfileNotFoundException", e.Message);
                return View("../Home/Index", model);
            }
            catch (NetflixLoginException e)
            {
                ModelState.AddModelError("NetflixLoginException", $"There was a problem while login in to your Netflix account: {e.Message}");
                return View("../Home/Index", model);
            }

            var mappedItems = _mapper.Map<List<Models.EntityFrameworkModels.NetflixViewedItem>>(viewedItems);

            var identificationGuid = await _netflixViewedItemRepository.CreateManyWithGuidAsync(mappedItems, model.NetflixProfileName);

            return RedirectToAction("Overview", "Stats", new { id = identificationGuid });
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

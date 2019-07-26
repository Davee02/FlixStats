using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DaHo.Library.AspNetCore.CustomAttributes;
using FlixStats.Data.Repositories.Abstractions;
using FlixStats.Models.InputModels;
using FlixStats.Models.ViewModels;
using FlixStats.Services.Abstractions;
using FlixStats.Stats;
using FlixStats.Stats.Model;
using Microsoft.AspNetCore.Mvc;

namespace FlixStats.Controllers
{
    public class StatsController : Controller
    {
        private readonly INetflixViewedItemRepository _netflixViewedItemRepository;
        private readonly IMapper _mapper;
        private readonly INetflixStatsCreator _netflixStatsCreator;
        private readonly IEnumerable<INetflixViewedItemsFileExporter> _netlNetflixViewedItemsFileExporters;

        public StatsController(
            INetflixViewedItemRepository netflixViewedItemRepository,
            IMapper mapper,
            INetflixStatsCreator netflixStatsCreator,
            IEnumerable<INetflixViewedItemsFileExporter> netlNetflixViewedItemsFileExporters)
        {
            _netflixViewedItemRepository = netflixViewedItemRepository;
            _mapper = mapper;
            _netflixStatsCreator = netflixStatsCreator;
            _netlNetflixViewedItemsFileExporters = netlNetflixViewedItemsFileExporters;
        }


        public IActionResult Index(NetflixStatsViewModel model)
        {
            return View("Index", model);
        }


        [Route("stats/overview/{identifier:guid}")]
        public async Task<IActionResult> GetPlaybacksForIdentifier(Guid identifier)
        {
            var viewedItems = await _netflixViewedItemRepository.GetByGuidAsync(identifier);
            if (!viewedItems?.Any() ?? false)
                return RedirectToAction("NoResults");

            var playbacks =
                NetflixViewingHistoryLoader.GetNetflixPlaybacksFromViewingActivity(
                    _mapper.Map<List<NetflixViewedItem>>(viewedItems));

            var viewModel = _netflixStatsCreator.GetNetflixStatsViewModel(playbacks);
            viewModel.Identifier = identifier;
            viewModel.ResultsAreKept = viewedItems.FirstOrDefault().KeepResult;
            viewModel.QueryDateTime = viewedItems.FirstOrDefault().SavedDateTime;

            return View("Index", viewModel);
        }

        [Route("stats/ajax/playbacks/{identifier:guid}")]
        [AjaxOnly]
        public async Task<IActionResult> GetPlaybacksForIdentifierOnDate(Guid identifier, DateTime forDate)
        {
            var viewedItems = await _netflixViewedItemRepository.GetByGuidForDayAsync(identifier, forDate);

            var playbacks =
                NetflixViewingHistoryLoader.GetNetflixPlaybacksFromViewingActivity(
                    _mapper.Map<List<NetflixViewedItem>>(viewedItems));

            return PartialView("Partials/_PlaybacksPartial", _netflixStatsCreator.GetNetflixPlaybacksViewModel(playbacks));
        }

       
        [Route("stats/export/{identifier:guid}")]
        public async Task<IActionResult> Export(ExportInputModel model)
        {
            var viewedItems = (await _netflixViewedItemRepository.GetByGuidAsync(model.Identifier))
                ?.OrderByDescending(x => x.PlaybackDateTime);

            if (!viewedItems?.Any() ?? false)
                return RedirectToAction("NoResults");

            var fileExporter =
                _netlNetflixViewedItemsFileExporters.FirstOrDefault(x => x.IsFormatSupported(model.Format));

            if (fileExporter == null)
            {
                return BadRequest($"Unknown format: {model.Format}");
            }

            fileExporter.ViewedItems = _mapper.Map<IEnumerable<Models.ImportExportModels.NetflixViewedItem>>(viewedItems);

            return File(fileExporter.GetFileContent(), fileExporter.GetMimeType(), fileExporter.GetFileName());
        }

        [Route("stats/ajax/keepResults/{identifier:guid}")]
        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KeepResults(Guid identifier)
        {
            var viewedItems = await _netflixViewedItemRepository.GetByGuidAsync(identifier);
            if (!viewedItems?.Any() ?? false)
                return RedirectToAction("NoResults");

            await _netflixViewedItemRepository.SetKeepResultsStateAsync(identifier);

            return Ok();
        }


        public IActionResult NoResults()
        {
            Response.StatusCode = 404;
            return View("NoResults");
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using NetflixStatizier.Stats;
using NetflixStatizier.Stats.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DaHo.Library.AspNetCore.CustomAttributes;
using NetflixStatizier.Data.Repositories.Abstractions;
using NetflixStatizier.Models.InputModels;
using NetflixStatizier.Models.ViewModels;
using NetflixStatizier.Services.Abstractions;

namespace NetflixStatizier.Controllers
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
            if (viewedItems == null)
                return NotFound($"There are no results saved with the identifier {identifier}");

            var playbacks =
                NetflixViewingHistoryLoader.GetNetflixPlaybacksFromViewingActivity(
                    _mapper.Map<List<NetflixViewedItem>>(viewedItems));
            var viewModel = _netflixStatsCreator.GetNetflixStatsViewModel(playbacks);
            viewModel.Identifier = identifier;

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

            return PartialView("_PlaybacksPartial", _netflixStatsCreator.GetNetflixPlaybacksViewModel(playbacks));
        }

       
        [Route("stats/export/{identifier:guid}")]
        [ActionName("export")]
        public async Task<IActionResult> ExportPlaybacks(ExportInputModel model)
        {
            var viewedItems = (await _netflixViewedItemRepository.GetByGuidAsync(model.Identifier))
                ?.OrderByDescending(x => x.PlaybackDateTime);

            if (viewedItems == null)
                return NotFound($"There are no results saved with the identifier {model.Identifier}");

            var fileExporter =
                _netlNetflixViewedItemsFileExporters.FirstOrDefault(x => x.IsFormatSupported(model.Format));

            if (fileExporter == null)
            {
                return BadRequest($"Unknown format: {model.Format}");
            }

            fileExporter.ViewedItems = _mapper.Map<IEnumerable<Models.ImportExportModels.NetflixViewedItem>>(viewedItems);

            return File(fileExporter.GetFileContent(), fileExporter.GetMimeType(), fileExporter.GetFileName());
        }
    }
}
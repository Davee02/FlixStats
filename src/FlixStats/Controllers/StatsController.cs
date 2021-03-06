﻿using System;
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
using Microsoft.AspNetCore.Mvc;
using NetflixViewedItem = FlixStats.Stats.Model.NetflixViewedItem;

namespace FlixStats.Controllers
{
    public class StatsController : Controller
    {
        private readonly INetflixViewedItemRepository _netflixViewedItemRepository;
        private readonly ILeaderboardRepository _leaderboardRepository;
        private readonly IQueryResultRepository _queryResultRepository;
        private readonly IMapper _mapper;
        private readonly INetflixStatsCreator _netflixStatsCreator;
        private readonly IEnumerable<INetflixViewedItemsFileExporter> _netlNetflixViewedItemsFileExporters;

        public StatsController(
            INetflixViewedItemRepository netflixViewedItemRepository,
            ILeaderboardRepository leaderboardRepository,
            IQueryResultRepository queryResultRepository,
            IMapper mapper,
            INetflixStatsCreator netflixStatsCreator,
            IEnumerable<INetflixViewedItemsFileExporter> netlNetflixViewedItemsFileExporters)
        {
            _netflixViewedItemRepository = netflixViewedItemRepository;
            _leaderboardRepository = leaderboardRepository;
            _queryResultRepository = queryResultRepository;
            _mapper = mapper;
            _netflixStatsCreator = netflixStatsCreator;
            _netlNetflixViewedItemsFileExporters = netlNetflixViewedItemsFileExporters;
        }


        public IActionResult Index(NetflixStatsViewModel model)
        {
            return View("Index", model);
        }


        [Route("stats/overview/{identifier:guid}")]
        public async Task<IActionResult> Overview(Guid identifier)
        {
            var viewedItems = await _netflixViewedItemRepository.GetByGuidAsync(identifier);
            if (!viewedItems?.Any() ?? true)
                return RedirectToAction("NoResults");

            var playbacks =
                NetflixViewingHistoryLoader.GetNetflixPlaybacksFromViewingActivity(
                    _mapper.Map<List<NetflixViewedItem>>(viewedItems));

            var viewModel = _netflixStatsCreator.GetNetflixStatsViewModel(playbacks);
            viewModel.Identifier = identifier;

            var queryResult = await _queryResultRepository.GetByGuidWithoutViewedItemsAsync(identifier);
            viewModel.ResultsAreKept = queryResult.KeepResults;
            viewModel.IsPublishedToLeaderboard = queryResult.IsPublishedToLeaderboard;
            viewModel.QueryDateTime = queryResult.QueryDateTime;
            viewModel.ForProfile = queryResult.ForProfile;

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

            if (!viewedItems?.Any() ?? true)
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

        [Route("stats/ajax/addToLeaderboard/{identifier:guid}")]
        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToLeaderboard(Guid identifier, LeaderboardInputModel inputModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _leaderboardRepository.CreateItemAsync(
                inputModel.Username,
                inputModel.CountryCode,
                await _netflixViewedItemRepository.GetTotalPlaybackTimeAsync(identifier));
            await _netflixViewedItemRepository.SetPublishedToLeaderboardStateAsync(identifier);

            return Ok();
        }

        public IActionResult NoResults()
        {
            Response.StatusCode = 404;
            return View("NoResults");
        }
    }
}
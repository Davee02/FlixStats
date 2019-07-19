﻿using Microsoft.AspNetCore.Mvc;
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
using NetflixStatizier.Stats.Exceptions;

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
                return BadRequest($"There are no results saved with the identifier {identifier}");

            var playbacks =
                NetflixViewingHistoryLoader.GetNetflixPlaybacksFromViewingActivity(
                    _mapper.Map<List<NetflixViewedItem>>(viewedItems));
            var viewModel = _netflixStatsCreator.GetNetflixStatsViewModel(playbacks);
            viewModel.Identifier = identifier;

            return View("Index", viewModel);
        }

        [Route("stats/partial/date/{identifier:guid}")]
        [AjaxOnly]
        public async Task<IActionResult> GetPlaybacksForIdentifierOnDate(Guid identifier, DateTime forDate)
        {
            var viewedItems = await _netflixViewedItemRepository.GetByGuidForDayAsync(identifier, forDate);

            var playbacks =
                NetflixViewingHistoryLoader.GetNetflixPlaybacksFromViewingActivity(
                    _mapper.Map<List<NetflixViewedItem>>(viewedItems));

            return Json(playbacks.Select(x => $"{x.Episode.Serie.Title}: {x.Episode.Title}").ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Overview(NetflixAccountInputModel model)
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

            var identificationGuid = Guid.NewGuid();
            var mappedItems = _mapper.Map<List<Models.EntityFrameworkModels.NetflixViewedItem>>(viewedItems);

            mappedItems.ForEach(x => x.Identifier = identificationGuid);

            await _netflixViewedItemRepository.CreateManyAsync(mappedItems);

            return RedirectToAction("Overview", new { id = identificationGuid });
        }

        [Route("stats/export/{identifier:guid}")]
        [ActionName("export")]
        public async Task<IActionResult> ExportPlaybacks(ExportInputModel model)
        {
            var viewedItems = (await _netflixViewedItemRepository.GetByGuidAsync(model.Identifier))
                ?.OrderByDescending(x => x.PlaybackDateTime);

            if (viewedItems == null)
                return BadRequest($"There are no results saved with the identifier {model.Identifier}");

            var fileExporter =
                _netlNetflixViewedItemsFileExporters.FirstOrDefault(x => x.IsFormatSupported(model.Format));

            if (fileExporter == null)
            {
                return BadRequest($"Unknown format: {model.Format}");
            }

            fileExporter.ViewedItems = viewedItems;

            return File(fileExporter.GetFileContent(), fileExporter.GetMimeType(), fileExporter.GetFileName());
        }
    }
}
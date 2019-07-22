﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetflixStatizier.Stats.Model;

namespace NetflixStatizier.Models.ImportExportModels
{
    [Serializable]
    public class NetflixViewedItem
    {
        public string Title { get; set; }

        public string VideoTitle { get; set; }

        public int MovieId { get; set; }

        public string CountryCode { get; set; }

        public int PlaybackBookmark { get; set; }

        public int Duration { get; set; }

        public DateTime PlaybackDateTime { get; set; }

        public Enums.DeviceType DeviceType { get; set; }

        public int Index { get; set; }

        public string TopNodeId { get; set; }

        public int SeriesId { get; set; }

        public string SeriesTitle { get; set; }

        public string SeasonDescriptor { get; set; }

        public string EpisodeTitle { get; set; }

        public int EstRating { get; set; }
    }
}
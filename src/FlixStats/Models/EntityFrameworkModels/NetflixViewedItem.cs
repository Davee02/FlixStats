using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FlixStats.Stats.Model;

namespace FlixStats.Models.EntityFrameworkModels
{
    public class NetflixViewedItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public Guid Identifier { get; set; }

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

        public DateTime SavedDateTime { get; set; }

        public bool KeepResult { get; set; }
    }
}
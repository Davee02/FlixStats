using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlixStats.Models.EntityFrameworkModels
{
    public class LeaderboardItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string Username { get; set; }

        public int TotalPlaybackTime { get; set; }

        public DateTime PublisheDateTime { get; set; }

        public string UserCountry { get; set; }
    }
}

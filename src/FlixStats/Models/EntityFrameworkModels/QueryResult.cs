using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlixStats.Models.EntityFrameworkModels
{
    public class QueryResult
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public ICollection<NetflixViewedItem> NetflixViewedItems { get; set; }

        public Guid Identifier { get; set; }

        public DateTime QueryDateTime { get; set; }

        public bool KeepResults { get; set; }

        public bool IsPublishedToLeaderboard { get; set; }

        public string ForProfile { get; set; }
    }
}

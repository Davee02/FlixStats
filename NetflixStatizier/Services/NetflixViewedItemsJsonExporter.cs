using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetflixStatizier.Models.EntityFrameworkModels;
using NetflixStatizier.Services.Abstractions;
using Newtonsoft.Json;

namespace NetflixStatizier.Services
{
    public class NetflixViewedItemsJsonExporter : INetflixViewedItemsFileExporter
    {
        private readonly List<NetflixViewedItem> _viewedItems;

        public NetflixViewedItemsJsonExporter(IEnumerable<NetflixViewedItem> viewedItems)
        {
            _viewedItems = viewedItems.ToList();
        }

        public byte[] GetFileContent()
        {
            var json = JsonConvert.SerializeObject(_viewedItems, Formatting.Indented);

            return Encoding.UTF8.GetBytes(json);
        }

        public string GetFileName() => $"TWON-export-{DateTime.Now:yyyyMMddhhmmss}.json";

        public string GetMimeType() => "application/json";
    }
}

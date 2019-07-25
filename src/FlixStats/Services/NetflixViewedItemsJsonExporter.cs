using System;
using System.Collections.Generic;
using System.Text;
using FlixStats.Models.ImportExportModels;
using FlixStats.Services.Abstractions;
using Newtonsoft.Json;

namespace FlixStats.Services
{
    public class NetflixViewedItemsJsonExporter : INetflixViewedItemsFileExporter
    {
        public IEnumerable<NetflixViewedItem> ViewedItems { get; set; }

        public byte[] GetFileContent()
        {
            var json = JsonConvert.SerializeObject(ViewedItems, Formatting.Indented);

            return Encoding.UTF8.GetBytes(json);
        }

        public string GetFileName() => $"FlixStats-export-{DateTime.Now:yyyyMMddhhmmss}.json";

        public string GetMimeType() => "application/json";

        public bool IsFormatSupported(string format) =>
            string.Equals(format, "json", StringComparison.OrdinalIgnoreCase);
    }
}

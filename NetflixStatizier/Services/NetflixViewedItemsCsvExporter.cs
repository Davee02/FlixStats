using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using NetflixStatizier.Models.EntityFrameworkModels;
using NetflixStatizier.Services.Abstractions;

namespace NetflixStatizier.Services
{
    public class NetflixViewedItemsCsvExporter : INetflixViewedItemsFileExporter
    {
        private readonly List<NetflixViewedItem> _viewedItems;

        public NetflixViewedItemsCsvExporter(IEnumerable<NetflixViewedItem> viewedItems)
        {
            _viewedItems = viewedItems.ToList();
        }

        public byte[] GetFileContent()
        {
            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream, Encoding.UTF8);
            var csvWriter = new CsvWriter(streamWriter, new Configuration { SanitizeForInjection = true });

            csvWriter.WriteRecords(_viewedItems);
            csvWriter.Flush();

            return stream.ToArray();
        }

        public string GetFileName() => $"TWON-export-{DateTime.Now:yyyyMMddhhmmss}.csv";

        public string GetMimeType() => "text/csv";
    }
}

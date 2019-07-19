using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using NetflixStatizier.Models.EntityFrameworkModels;
using NetflixStatizier.Services.Abstractions;

namespace NetflixStatizier.Services
{
    public class NetflixViewedItemsCsvExporter : INetflixViewedItemsFileExporter
    {
        public IEnumerable<NetflixViewedItem> ViewedItems { get; set; }

        public byte[] GetFileContent()
        {
            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream, Encoding.UTF8);
            var csvWriter = new CsvWriter(streamWriter, new Configuration { SanitizeForInjection = true });

            csvWriter.WriteRecords(ViewedItems);
            csvWriter.Flush();

            return stream.ToArray();
        }

        public string GetFileName() => $"FlixStats-export-{DateTime.Now:yyyyMMddhhmmss}.csv";

        public string GetMimeType() => "text/csv";

        public bool IsFormatSupported(string format) =>
            string.Equals(format, "csv", StringComparison.OrdinalIgnoreCase);
    }
}

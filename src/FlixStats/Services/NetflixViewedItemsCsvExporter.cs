﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using CsvHelper;
using FlixStats.Models.ImportExportModels;
using FlixStats.Services.Abstractions;

namespace FlixStats.Services
{
    public class NetflixViewedItemsCsvExporter : INetflixViewedItemsFileExporter
    {
        public IEnumerable<NetflixViewedItem> ViewedItems { get; set; }

        public byte[] GetFileContent()
        {
            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream, Encoding.UTF8);
            var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);

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

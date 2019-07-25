using System.Collections.Generic;
using FlixStats.Models.ImportExportModels;

namespace FlixStats.Services.Abstractions
{
    public interface INetflixViewedItemsFileExporter
    {
        IEnumerable<NetflixViewedItem> ViewedItems { get; set; }

        byte[] GetFileContent();

        string GetFileName();

        string GetMimeType();

        bool IsFormatSupported(string format);
    }
}

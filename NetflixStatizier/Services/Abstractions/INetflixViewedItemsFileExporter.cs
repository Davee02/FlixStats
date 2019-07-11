using System.Collections.Generic;
using NetflixStatizier.Models.EntityFrameworkModels;

namespace NetflixStatizier.Services.Abstractions
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

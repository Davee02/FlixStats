using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using NetflixStatizier.Models.ImportExportModels;
using NetflixStatizier.Services.Abstractions;

namespace NetflixStatizier.Services
{
    public class NetflixViewedItemsXmlExporter : INetflixViewedItemsFileExporter
    {
        public IEnumerable<NetflixViewedItem> ViewedItems { get; set; }

        public byte[] GetFileContent()
        {
            using (var stringwriter = new System.IO.StringWriter())
            {
                var serializer = new XmlSerializer(typeof(List<NetflixViewedItem>));
                serializer.Serialize(stringwriter, ViewedItems.ToList());
                return Encoding.UTF8.GetBytes(stringwriter.ToString());
            }
        }

        public string GetFileName() => $"FlixStats-export-{DateTime.Now:yyyyMMddhhmmss}.xml";

        public string GetMimeType() => "text/xml";

        public bool IsFormatSupported(string format) =>
            string.Equals(format, "xml", StringComparison.OrdinalIgnoreCase);
    }
}

using System;

namespace FlixStats.Models.InputModels
{
    public class ExportInputModel
    {
        public Guid Identifier { get; set; }

        public string Format { get; set; }
    }
}

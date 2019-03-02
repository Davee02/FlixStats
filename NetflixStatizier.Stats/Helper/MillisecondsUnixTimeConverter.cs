using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NetflixStatizier.Stats.Helper
{
    public class MillisecondsUnixTimeConverter : DateTimeConverterBase
    {
        private static readonly DateTime m_Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteRawValue(((DateTime)value - m_Epoch).TotalMilliseconds + "000");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null) { return null; }
            return m_Epoch.AddMilliseconds((long)reader.Value / 1000d);
        }
    }
}
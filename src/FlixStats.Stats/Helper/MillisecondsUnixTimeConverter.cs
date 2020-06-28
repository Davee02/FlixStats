using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FlixStats.Stats.Helper
{
    public class MillisecondsUnixTimeConverter : DateTimeConverterBase
    {
        private readonly DateTime _epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteRawValue(((DateTime)value - _epochStart).TotalMilliseconds.ToString(new CultureInfo("en-us")));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
                return null;
            return _epochStart
                .AddMilliseconds((long)reader.Value)
                .ToLocalTime();
        }
    }
}
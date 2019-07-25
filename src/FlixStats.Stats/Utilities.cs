using System;
using Newtonsoft.Json.Linq;

namespace FlixStats.Stats
{
    public static class Utilities
    {
        internal static bool IsValidJson(this string json)
        {
            try
            {
                JToken.Parse(json);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

using Newtonsoft.Json.Linq;
using System;

namespace NetflixStatizier.Stats
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

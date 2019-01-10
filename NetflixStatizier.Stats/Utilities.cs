using Newtonsoft.Json.Linq;
using System;

namespace NetflixStatizier.Stats
{
    static class Utilities
    {
        public static bool IsValidJson(this string json)
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

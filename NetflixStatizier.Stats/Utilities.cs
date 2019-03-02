using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;

namespace NetflixStatizier.Stats
{
    public static class Utilities
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

        public static string GetKeyValueStringOutOfCookieCollection(ICollection<Cookie> cookies)
        {
            var sb = new StringBuilder();
            foreach (var cookie in cookies)
            {
                sb.Append($"{cookie.Name}={cookie.Value};");
            }

            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
    }
}

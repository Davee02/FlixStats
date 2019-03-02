using Microsoft.AspNetCore.Mvc;
using NetflixStatizier.Models;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using NetflixStatizier.Stats;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace NetflixStatizier.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetStats(NetflixAccountModel model)
        {
            using (var driver = GetWebDriver())
            {
                //var stats = new NetflixStats("kiumo777@gmail.com", "s-INF17a+");
                var stats = new NetflixStats(model.NetflixEmail, model.NetflixPassword);
                var history = await stats.GetNetflixViewingHistory("dave", driver);
                return Ok(history);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private static IWebDriver GetWebDriver()
        {
            var options = new ChromeOptions();
            options.AddArgument("headless");
            options.AddArgument("blink-settings=imagesEnabled=false");
            options.AddArgument("disable-gpu");
            return new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), options);
        }
    }
}

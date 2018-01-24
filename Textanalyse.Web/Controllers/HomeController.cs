using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Textanalyse.Web.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;

namespace Textanalyse.Web.Controllers
{
    public class HomeController : Controller
    {
        readonly ILogger<HomeController> _log;

        readonly IStringLocalizer<HomeController> localizer;

        public HomeController(ILogger<HomeController> log, IStringLocalizer<HomeController> localizer)
        {
            _log = log;
            this.localizer = localizer;
        }

        [HttpGet("/")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("/search")]
        public IActionResult Search()
        {
            _log.LogInformation("Es wurde eine Suche gestartet");
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Textanalyse.Web.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;
using Textanalyse.Data.Repository;

namespace Textanalyse.Web.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        readonly ILogger<HomeController> _log;

        readonly IStringLocalizer<HomeController> localizer;

        readonly IRepository repository;

        public HomeController(ILogger<HomeController> log, IStringLocalizer<HomeController> localizer, IRepository repository)
        {
            _log = log;
            this.localizer = localizer;
            this.repository = repository;
        }

        [HttpGet("/")]
        public IActionResult Index()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                return View("LoggedIn");
            }

            return View();
        }

        [HttpGet("/search")]
        public IActionResult Search()
        {
            _log.LogInformation("Es wurde eine Suche gestartet");
            return View();
        }

        [HttpPost("/textSave")]
        public IActionResult TextSave(string text)
        {
            this.repository.SaveText(text);
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

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
using Textanalyse.Data.Data;

namespace Textanalyse.Web.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        readonly ILogger<HomeController> _log;

        readonly IStringLocalizer<HomeController> localizer;

        readonly IRepository repository;

        readonly TextContext context;

        readonly DbManager dbManager;

        public HomeController(ILogger<HomeController> log, IStringLocalizer<HomeController> localizer, IRepository repository, TextContext context)
        {
            _log = log;
            this.context = context;
            this.localizer = localizer;
            this.repository = repository;
            this.dbManager = new DbManager(context);
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
            if(!this.User.Identity.IsAuthenticated)
            {
                return View("~/Views/Shared/Error.cshtml");
            }

            this.repository.SaveText(text, this.User.Identity.Name);
            return View("LoggedIn");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

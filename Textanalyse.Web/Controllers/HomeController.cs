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
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;
using Textanalyse.Web.Entities;

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
                return View("~/Views/Home/LoggedIn.cshtml");
            }

            return View();
        }

        [HttpGet("/search")]
        public IActionResult Search(string searchterm)
        {
            if (string.IsNullOrWhiteSpace(searchterm))
            {
                return View();
            }

            string[] searchterms = searchterm.Split(' ');

            if (searchterms.Length > 3)
            {
                return View();
            }

            List<TextResult> searchResult = repository.SearchResult(searchterm);

            JArray results = new JArray();

            JObject jsonobject = new JObject();

            jsonobject.Add("results", results);

            for (int i = 0; i < searchResult.Count; i++)
            {
                JArray section = new JArray();
                JObject text = new JObject();
                text.Add("textid", JToken.FromObject(searchResult[i].TextID));
                text.Add("scoreTotal", JToken.FromObject(searchResult[i].Score));
                text.Add("sections", section);

                for (int j = 0; j < searchResult[i].Sentences.Count; j++)
                {
                    JObject sentence = new JObject();
                    sentence.Add("sentenceID", JToken.FromObject(searchResult[i].Sentences[j].SentenceID));
                    sentence.Add("score", JToken.FromObject(searchResult[i].Sentences[j].Score));
                    sentence.Add("scoreSummary", JToken.FromObject(searchResult[i].Sentences[j].Summary.ToArray()));
                    section.Add(sentence);
                }

                results.Add(text);
            }
            TempData["result"] = jsonobject.ToString();
            _log.LogInformation("Search started.");

            if (User.Identity.IsAuthenticated)
            {
                return View("~/Views/Home/LoggedIn.cshtml");
            }
            else
            {
                return View("~/Views/Home/Index.cshtml");
            }
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

        [HttpPost("/summary")]
        public IActionResult Summary()
        {
            if (User.Identity.IsAuthenticated)
            {
                List<Text> texts = repository.GetTexts();
                this.TempData["texts"] = texts;
                return View("~/Views/Home/Summary.cshtml");
            }
            else
            {
                return View("~/Views/Home/Index.cshtml");
            }
        }

        public IActionResult Error()
        {
            this._log.LogError("An Error occured.");
            return View("~/Views/Error/Error.cshtml");
        }
    }
}

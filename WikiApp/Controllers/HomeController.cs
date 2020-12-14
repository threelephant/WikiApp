using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WikiApp.Models;
using WikiApp.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace WikiApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly WikiContext db;
        private readonly ILogger<HomeController> logger;
        
        public HomeController(WikiContext db, ILogger<HomeController> logger)
        {
            this.db = db;
            this.logger = logger;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            var words = db.Словаs.ToList()
                .GroupBy(w => w.Слово.Substring(0, 1).ToUpper(),
                    (alphabet, sublist) =>
                        new WordsByFirstLetter
                        {
                            Alphabet = alphabet, 
                            SubList = sublist.OrderBy(x => x.Слово).ToList()
                        })
                .OrderBy(x => x.Alphabet).ToList();

            ViewBag.Words = words;
            
            return View();
        }
    
        [AllowAnonymous]
        public IActionResult Word(Guid id)
        {
            var articles = from history in db.ИсторияПравокСтатьиs
                join status in db.СтатусыПравокСтатьиs on history.IdСтатуса equals status.IdСтатуса
                where status.Наименование == "Одобрено"
                group history by history.IdСтатьи
                into g
                select new
                {
                    ArticleId = g.Key,
                    MaxDate = g.Max(h => h.ДатаРассмотрения)
                };

            var words = from word in db.Словаs
                join article in db.Статьиs on word.IdСлова equals article.IdСлова
                join history in db.ИсторияПравокСтатьиs on article.IdСтатьи equals history.IdСтатьи
                join status in db.СтатусыПравокСтатьиs on history.IdСтатуса equals status.IdСтатуса
                where status.Наименование == "Одобрено" && word.IdСлова == id
                select new
                {
                    Word = word.Слово,
                    Text = history.Текст,
                    ArticleId = history.IdСтатьи,
                    Date = history.ДатаРассмотрения
                };

            var groups = from article in articles
                join word in words on article.MaxDate equals word.Date
                join article2 in articles on word.ArticleId equals article2.ArticleId
                select new WordArticle
                {
                    Word = word.Word,
                    Text = word.Text,
                    Id = article2.ArticleId
                };

            ViewBag.WordArticles = groups.ToList();
            
            return View();
        }

        public IActionResult History(Guid id)
        {
            var history = from his in db.ИсторияПравокСтатьиs
                join status in db.СтатусыПравокСтатьиs on his.IdСтатуса equals status.IdСтатуса
                where status.Наименование == "Одобрено" && his.IdСтатьи == id
                orderby his.ДатаРассмотрения
                select new HistoryArticle
                {
                    Article = his.Текст,
                    DateRequest = his.ДатаЗаявки,
                    Author = his.Логин,
                    Moderator = his.ЛогинМодератора,
                    Date = his.ДатаРассмотрения
                };

            ViewBag.History = history.ToList();

            return View();
        }

        public IActionResult Stats()
        {
            return null;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

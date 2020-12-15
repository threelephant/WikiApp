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
            var filteredWords = from word in db.Словаs
                join article in db.Статьиs on word.IdСлова equals article.IdСлова
                join history in db.ИсторияПравокСтатьиs on article.IdСтатьи equals history.IdСтатьи
                join status in db.СтатусыПравокСтатьиs on history.IdСтатуса equals status.IdСтатуса
                where status.Наименование == "Одобрено"
                select word;
            
            var words = filteredWords.Distinct().ToList()
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
            if (!IsAnyApprovedArticles(id))
            {
                return RedirectToAction("Index", "Home");
            }
            
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
                join article in db.Статьиs on his.IdСтатьи equals article.IdСтатьи
                join word in db.Словаs on article.IdСлова equals word.IdСлова
                where status.Наименование == "Одобрено" && his.IdСтатьи == id
                orderby his.ДатаРассмотрения descending
                select new HistoryArticle
                {
                    Article = his.Текст,
                    WordId = word.IdСлова,
                    Word = word.Слово,
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
            var requests = db.Пользователиs.Join(db.ИсторияПравокСтатьиs,
                    u => u.Логин,
                    h => h.Логин,
                    (u, h) => new {u, h})
                .Select(r => new
                {
                    User = r.u.Логин,
                    Action = "Заявление на правку",
                    Date = r.h.ДатаЗаявки
                });
            
            var acceptedRequests = db.Пользователиs.Join(db.ИсторияПравокСтатьиs,
                    u => u.Логин,
                    h => h.Логин,
                    (u, h) => new {u, h})
                .Join(db.СтатусыПравокСтатьиs,
                    uh => uh.h.IdСтатуса,
                    s => s.IdСтатуса,
                    (uh, s) => new {uh, s})
                .Where(uhs => uhs.s.Наименование == "Одобрено")
                .Select(r => new
                {
                    User = r.uh.u.Логин,
                    Action = "Принятые правки",
                    Date = r.uh.h.ДатаРассмотрения.GetValueOrDefault()
                });
            
            var moderators = db.Пользователиs.Join(db.МодераторСтатьиs,
                    u => u.Логин,
                    m => m.Логин,
                    (u, m) => new {u, m})
                .Join(db.СтатусыЗаявокНаМодерациюs,
                    uh => uh.m.IdСтатуса,
                    s => s.IdСтатуса,
                    (uh, s) => new {uh, s})
                .Where(uhs => uhs.s.Наименование == "Одобрено")
                .Select(r => new
                {
                    User = r.uh.u.Логин,
                    Action = "Модератор",
                    Date = r.uh.m.ДатаЗаявки
                });

            var statsDates = requests.AsEnumerable()
                .Union(acceptedRequests.AsEnumerable())
                .Union(moderators.AsEnumerable());

            var stats = statsDates.Where(s =>
                s.Date >= DateTime.Now.AddYears(-5)).GroupBy(g =>
                new {g.User, g.Date.Month, g.Date.Year}).Select(s => new StatsModel
            {
                User = s.Key.User,
                Year = s.Key.Year,
                Month = s.Key.Month,
                Requests = s.Count(r => r.Action == "Заявление на правку"),
                AcceptedRequests = s.Count(r => r.Action == "Принятые правки"),
                Moderators = s.Count(r => r.Action == "Модератор")
            })
                .OrderBy(s => s.Year)
                .ThenBy(s => s.Month);

            return View(stats.ToList());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private bool IsAnyApprovedArticles(Guid wordId)
        {
            var articles = from word in db.Словаs
                join article in db.Статьиs on word.IdСлова equals article.IdСлова
                join history in db.ИсторияПравокСтатьиs on article.IdСтатьи equals history.IdСтатьи
                join status in db.СтатусыПравокСтатьиs on history.IdСтатуса equals status.IdСтатуса
                where status.Наименование == "Одобрено" && word.IdСлова == wordId
                select new
                {
                    history.IdПравки
                };

            if (!articles.Any())
            {
                return false;
            }
            
            return true;
        }
    }
}

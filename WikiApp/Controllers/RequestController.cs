using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WikiApp.Models;
using WikiApp.ViewModels;

namespace WikiApp.Controllers
{
    [Authorize]
    public class RequestController : Controller
    {
        private readonly WikiContext db;
        private readonly ILogger<RequestController> logger;
        
        public RequestController(WikiContext db, ILogger<RequestController> logger)
        {
            this.db = db;
            this.logger = logger;
        }
        
        [HttpGet]
        public IActionResult Add(Guid id)
        {
            var history = from his in db.ИсторияПравокСтатьиs
                join status in db.СтатусыПравокСтатьиs on his.IdСтатуса equals status.IdСтатуса
                join article in db.Статьиs on his.IdСтатьи equals article.IdСтатьи
                join word in db.Словаs on article.IdСлова equals word.IdСлова
                where status.Наименование == "Одобрено" && his.IdСтатьи == id
                orderby his.ДатаРассмотрения descending
                select new HistoryArticle
                {
                    Id = his.IdПравки,
                    Article = his.Текст,
                    WordId = word.IdСлова,
                    Word = word.Слово,
                    DateRequest = his.ДатаЗаявки,
                    Author = his.Логин,
                    Moderator = his.ЛогинМодератора,
                    Date = his.ДатаРассмотрения
                };

            ViewBag.lastChange = history.FirstOrDefault();
            
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Guid id, string text)
        {
            var change = new ИсторияПравокСтатьи
            {
                Логин = User.Identity.Name,
                IdСтатуса = db.СтатусыПравокСтатьиs.FirstOrDefault(s => s.Наименование == "Рассматривается").IdСтатуса,
                Текст = text,
                IdСтатьи = db.ИсторияПравокСтатьиs.FirstOrDefault(h => h.IdПравки == id).IdСтатьи,
                ДатаЗаявки = DateTime.Now,
                FkIdПравки = id,
            };

            db.ИсторияПравокСтатьиs.Add(change);
            db.SaveChanges();
            
            return RedirectToAction("UserRequests", "Request");
        }
        
        public IActionResult UserRequests()
        {
            var requests = db.ИсторияПравокСтатьиs.Where(h =>
                    h.IdСтатуса == db.СтатусыПравокСтатьиs.FirstOrDefault(s => 
                        s.Наименование == "Рассматривается").IdСтатуса).Where(h => 
                    h.Логин == User.Identity.Name).Include(h => h.IdСтатьиNavigation)
                .ThenInclude(a => a.IdСловаNavigation)
                .Select(r => new Requests
                {
                    Id = r.IdПравки,
                    WordId = r.IdСтатьиNavigation.IdСловаNavigation.IdСлова,
                    Word = r.IdСтатьиNavigation.IdСловаNavigation.Слово,
                    ArticleId = r.IdСтатьиNavigation.IdСтатьи,
                    Text = r.Текст,
                    DateRequest = r.ДатаЗаявки
                })
                .OrderByDescending(r => r.DateRequest);

            return View(requests.ToList());
        }

        public IActionResult Update(Guid id)
        {
            var change = db.ИсторияПравокСтатьиs.FirstOrDefault(h => h.IdПравки == id);

            if (change == null || change?.Логин != User.Identity.Name)
            {
                return NotFound();
            }
            
            var changedText = db.ИсторияПравокСтатьиs.FirstOrDefault(h => h.IdПравки == change.FkIdПравки)?.Текст;

            ViewBag.ChangedText = changedText;
            ViewBag.Id = change?.IdПравки;
            ViewBag.Text = change?.Текст;
            
            return View(change);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Guid id, string text)
        {
            var change = db.ИсторияПравокСтатьиs.FirstOrDefault(h => h.IdПравки == id);
            
            if (change == null || change?.Логин != User.Identity.Name)
            {
                return NotFound();
            }
            
            change.Текст = text;

            db.SaveChanges();
            return RedirectToAction("UserRequests", "Request");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Guid id)
        {
            var deletedId = db.ИсторияПравокСтатьиs.Include(h => h.IdСтатусаNavigation).FirstOrDefault(h => 
                h.IdПравки == id);
            if (User.Identity.Name != deletedId?.Логин
                && deletedId?.IdСтатусаNavigation.Наименование != "Рассматривается")
            {
                return RedirectToAction("UserRequests", "Request");
            }

            db.ИсторияПравокСтатьиs.Remove(deletedId ?? throw new InvalidOperationException());
            db.SaveChanges();

            return RedirectToAction("UserRequests", "Request");
        }
    }
}
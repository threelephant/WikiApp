using System;

namespace WikiApp.ViewModels
{
    public class HistoryArticle
    {
        public string Article { get; set; }
        public string Author { get; set; }
        public string Moderator { get; set; }
        public DateTime DateRequest { get; set; }
        public DateTime? Date { get; set; }
    }
}
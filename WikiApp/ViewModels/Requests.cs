using System;

namespace WikiApp.ViewModels
{
    public class Requests
    {
        public Guid Id { get; set; }
        public Guid WordId { get; set; }
        public string Word { get; set; }
        public Guid ArticleId { get; set; }
        public string Text { get; set; }
        public DateTime DateRequest { get; set; }
    }
}
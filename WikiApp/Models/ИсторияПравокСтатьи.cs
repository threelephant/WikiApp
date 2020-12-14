using System;
using System.Collections.Generic;

#nullable disable

namespace WikiApp.Models
{
    public partial class ИсторияПравокСтатьи
    {
        public ИсторияПравокСтатьи()
        {
            InverseFkIdПравкиNavigation = new HashSet<ИсторияПравокСтатьи>();
        }

        public Guid IdПравки { get; set; }
        public string Текст { get; set; }
        public string Логин { get; set; }
        public Guid IdСтатьи { get; set; }
        public Guid IdСтатуса { get; set; }
        public DateTime ДатаЗаявки { get; set; }
        public string ЛогинМодератора { get; set; }
        public Guid? IdМодерируемойСтатьи { get; set; }
        public DateTime? ДатаРассмотрения { get; set; }
        public Guid? FkIdПравки { get; set; }

        public virtual ИсторияПравокСтатьи FkIdПравкиNavigation { get; set; }
        public virtual СтатусыПравокСтатьи IdСтатусаNavigation { get; set; }
        public virtual Статьи IdСтатьиNavigation { get; set; }
        public virtual Пользователи ЛогинNavigation { get; set; }
        public virtual МодераторСтатьи МодераторСтатьи { get; set; }
        public virtual ICollection<ИсторияПравокСтатьи> InverseFkIdПравкиNavigation { get; set; }
    }
}

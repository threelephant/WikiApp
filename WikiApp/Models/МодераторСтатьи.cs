using System;
using System.Collections.Generic;

#nullable disable

namespace WikiApp.Models
{
    public partial class МодераторСтатьи
    {
        public МодераторСтатьи()
        {
            ИсторияПравокСтатьиs = new HashSet<ИсторияПравокСтатьи>();
        }

        public string Логин { get; set; }
        public Guid IdСтатьи { get; set; }
        public Guid IdСтатуса { get; set; }
        public string ЛогинАдминистратора { get; set; }
        public DateTime ДатаЗаявки { get; set; }
        public DateTime? ДатаРассмотрения { get; set; }

        public virtual СтатусыЗаявокНаМодерацию IdСтатусаNavigation { get; set; }
        public virtual Статьи IdСтатьиNavigation { get; set; }
        public virtual Пользователи ЛогинNavigation { get; set; }
        public virtual Пользователи ЛогинАдминистратораNavigation { get; set; }
        public virtual ICollection<ИсторияПравокСтатьи> ИсторияПравокСтатьиs { get; set; }
    }
}

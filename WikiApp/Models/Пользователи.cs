using System;
using System.Collections.Generic;

#nullable disable

namespace WikiApp.Models
{
    public partial class Пользователи
    {
        public Пользователи()
        {
            ИсторияПравокСтатьиs = new HashSet<ИсторияПравокСтатьи>();
            МодераторСтатьиЛогинNavigations = new HashSet<МодераторСтатьи>();
            МодераторСтатьиЛогинАдминистратораNavigations = new HashSet<МодераторСтатьи>();
        }

        public string Логин { get; set; }
        public DateTime ДатаРегистрации { get; set; }
        public Guid IdРоли { get; set; }
        public string Пароль { get; set; }
        public Guid? Соль { get; set; }

        public virtual Роль IdРолиNavigation { get; set; }
        public virtual ICollection<ИсторияПравокСтатьи> ИсторияПравокСтатьиs { get; set; }
        public virtual ICollection<МодераторСтатьи> МодераторСтатьиЛогинNavigations { get; set; }
        public virtual ICollection<МодераторСтатьи> МодераторСтатьиЛогинАдминистратораNavigations { get; set; }
    }
}

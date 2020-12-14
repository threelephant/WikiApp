using System;
using System.Collections.Generic;

#nullable disable

namespace WikiApp.Models
{
    public partial class Роль
    {
        public Роль()
        {
            Пользователиs = new HashSet<Пользователи>();
        }

        public Guid IdРоли { get; set; }
        public string НаименованиеРоли { get; set; }

        public virtual ICollection<Пользователи> Пользователиs { get; set; }
    }
}

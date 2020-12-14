using System;
using System.Collections.Generic;

#nullable disable

namespace WikiApp.Models
{
    public partial class Статьи
    {
        public Статьи()
        {
            ИсторияПравокСтатьиs = new HashSet<ИсторияПравокСтатьи>();
            МодераторСтатьиs = new HashSet<МодераторСтатьи>();
        }

        public Guid IdСтатьи { get; set; }
        public Guid IdСлова { get; set; }

        public virtual Слова IdСловаNavigation { get; set; }
        public virtual ICollection<ИсторияПравокСтатьи> ИсторияПравокСтатьиs { get; set; }
        public virtual ICollection<МодераторСтатьи> МодераторСтатьиs { get; set; }
    }
}

using System;
using System.Collections.Generic;

#nullable disable

namespace WikiApp.Models
{
    public partial class СтатусыПравокСтатьи
    {
        public СтатусыПравокСтатьи()
        {
            ИсторияПравокСтатьиs = new HashSet<ИсторияПравокСтатьи>();
        }

        public Guid IdСтатуса { get; set; }
        public string Наименование { get; set; }

        public virtual ICollection<ИсторияПравокСтатьи> ИсторияПравокСтатьиs { get; set; }
    }
}

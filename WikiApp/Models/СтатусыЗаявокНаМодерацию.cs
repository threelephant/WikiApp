using System;
using System.Collections.Generic;

#nullable disable

namespace WikiApp.Models
{
    public partial class СтатусыЗаявокНаМодерацию
    {
        public СтатусыЗаявокНаМодерацию()
        {
            МодераторСтатьиs = new HashSet<МодераторСтатьи>();
        }

        public Guid IdСтатуса { get; set; }
        public string Наименование { get; set; }

        public virtual ICollection<МодераторСтатьи> МодераторСтатьиs { get; set; }
    }
}

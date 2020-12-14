using System;
using System.Collections.Generic;

#nullable disable

namespace WikiApp.Models
{
    public partial class Слова
    {
        public Слова()
        {
            Статьиs = new HashSet<Статьи>();
        }

        public Guid IdСлова { get; set; }
        public string Слово { get; set; }

        public virtual ICollection<Статьи> Статьиs { get; set; }
    }
}

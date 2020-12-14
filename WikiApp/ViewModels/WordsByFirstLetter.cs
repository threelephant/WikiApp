using System.Collections.Generic;
using WikiApp.Models;

namespace WikiApp.ViewModels
{
    public class WordsByFirstLetter
    {
        public string Alphabet { get; set; }
        public List<Слова> SubList { get; set; }
    }
}
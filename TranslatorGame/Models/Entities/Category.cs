using System;
using System.Collections.Generic;

namespace TranslatorGame.Models.Entities
{
    public class Category
    {
        public Guid Id { get; init; }
        public string Name { get; set; }
        public List<Word>? Words { get; set; } = new List<Word>();
        public override string ToString()
        {
            return ($"{Name}");
        }
    }
}

using System;
using System.Collections.Generic;

namespace TranslatorGame.Models.Entities
{
    public class Category
    {
        public Guid Id { get; init; }
        public string Name { get; set; }
        public List<Word>? Words { get; set; } = new List<Word>();

        public override bool Equals(object? obj)
        {
            return obj is Category category &&
                   Id.Equals(category.Id);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public override string ToString()
        {
            return ($"{Name}");
        }

        public static bool operator ==(Category? left, Category? right)
        {
            return EqualityComparer<Category>.Default.Equals(left, right);
        }

        public static bool operator !=(Category? left, Category? right)
        {
            return !(left == right);
        }
    }
}

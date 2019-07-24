using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace EnglishWords.Data.Entities
{
    public class Word : BaseEntity
    {
        [MaxLength(255)]
        public string English { get; set; }
        [MaxLength(255)]
        public string Russian { get; set; }
        [MaxLength(255)]
        public string Description { get; set; }

        public bool IsCorrect()
        {
            var areAnyEmpty = String.IsNullOrWhiteSpace(English) ||
                String.IsNullOrWhiteSpace(Russian) ||
                String.IsNullOrWhiteSpace(Description);
            if (areAnyEmpty)
                return false;
            var rusRegex = new Regex("^[а-яА-Я ]+$");
            var engRegex = new Regex("^[a-zA-Z ]+$");
            if (!rusRegex.IsMatch(Russian) || !engRegex.IsMatch(English))
                return false;
            return true;
        }

        public int GetUniqueKey() =>
            English.GetHashCode() ^ Russian.GetHashCode() ^ Description.GetHashCode();
    }
}

using System;

namespace ООП_2
{
    public class SearchResultItem
    {
        public string Category { get; set; } // Автор Книга или Издательство
        public string FieldName { get; set; }
        public string Value { get; set; }
        public string DisplayText { get; set; }

        public override string ToString()
        {
            return $"{Category}: {DisplayText}";
        }
    }
}
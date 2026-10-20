using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ООП_2
{
    public class AuthorNameAttribute : ValidationAttribute
    {
        private readonly bool _allowMultipleWords;
        private readonly int _minWords;

        public AuthorNameAttribute(bool allowMultipleWords = true, int minWords = 1)
        {
            _allowMultipleWords = allowMultipleWords;
            _minWords = minWords;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            string name = value.ToString();

            if (string.IsNullOrWhiteSpace(name))
            {
                return new ValidationResult("Имя не может быть пустым");
            }

            if (!Regex.IsMatch(name, @"^[а-яА-ЯёЁa-zA-Z\s\-]+$"))
            {
                return new ValidationResult("Имя может содержать только буквы, пробелы и дефисы");
            }

            if (_allowMultipleWords)
            {
                string[] words = name.Split(new char[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);

                if (words.Length < _minWords)
                {
                    return new ValidationResult($"Имя должно содержать не менее {_minWords} слов(а)");
                }

                foreach (string word in words)
                {
                    if (word.Length < 2)
                    {
                        return new ValidationResult($"Каждая часть имени должна содержать не менее 2 символов");
                    }
                }
            }

            if (char.IsLower(name[0]))
            {
                return new ValidationResult("Имя должно начинаться с заглавной буквы");
            }

            return ValidationResult.Success;
        }
    }
}
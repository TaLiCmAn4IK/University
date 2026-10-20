using System;
using System.ComponentModel.DataAnnotations;

namespace ООП_2
{
    public class PriceAttribute : ValidationAttribute
    {
        private readonly double _minPrice;
        private readonly double _maxPrice;
        private readonly bool _allowZero;

        // Меняем decimal на double
        public PriceAttribute(double minPrice = 0.01, double maxPrice = 100000, bool allowZero = false)
        {
            _minPrice = minPrice;
            _maxPrice = maxPrice;
            _allowZero = allowZero;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            decimal price;

            if (value is decimal decimalValue)
            {
                price = decimalValue;
            }
            else if (value is double doubleValue)
            {
                price = (decimal)doubleValue;
            }
            else if (value is int intValue)
            {
                price = intValue;
            }
            else
            {
                return new ValidationResult("Некорректный тип данных для цены");
            }

            // Конвертируем double в decimal для сравнения
            decimal minPrice = (decimal)_minPrice;
            decimal maxPrice = (decimal)_maxPrice;

            if (!_allowZero && price == 0)
            {
                return new ValidationResult("Цена не может быть равна нулю");
            }

            if (price < 0)
            {
                return new ValidationResult("Цена не может быть отрицательной");
            }

            if (price < minPrice || price > maxPrice)
            {
                return new ValidationResult($"Цена должна быть в диапазоне от {minPrice} до {maxPrice}");
            }

            if (decimal.Round(price, 2) != price)
            {
                return new ValidationResult("Цена может содержать не более 2 знаков после запятой");
            }

            return ValidationResult.Success;
        }
    }
}
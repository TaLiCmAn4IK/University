using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WatchShop.Entities
{
    /// <summary>
    /// Сущность "Категория" (Code First).
    /// Связь: одна категория → много часов (one-to-many).
    /// </summary>
    [Table("Categories")]
    public class CategoryEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = "";

        [MaxLength(50)]
        public string? NameKey { get; set; }

        public ICollection<WatchEntity> Watches { get; set; } = new List<WatchEntity>();
    }
}

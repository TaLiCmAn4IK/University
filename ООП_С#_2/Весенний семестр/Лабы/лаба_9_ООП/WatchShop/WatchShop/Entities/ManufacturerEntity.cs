using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WatchShop.Entities
{
    /// <summary>
    /// Сущность "Производитель" (Code First).
    /// Связь: один производитель → много часов (one-to-many).
    ///
    /// Аннотации (DataAnnotations) настраивают схему таблицы:
    ///   [Table]  — имя таблицы в БД
    ///   [Key]    — первичный ключ
    ///   [Required], [MaxLength] — NOT NULL и длина столбца
    /// </summary>
    [Table("Manufacturers")]
    public class ManufacturerEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = "";

        [MaxLength(50)]
        public string? Country { get; set; }

        public int? FoundedYear { get; set; }

        /// <summary>
        /// Навигационное свойство: список часов этого производителя.
        /// Через него EF понимает связь one-to-many и умеет
        /// подгружать связанные данные (Include / Lazy / Explicit loading).
        /// </summary>
        public ICollection<WatchEntity> Watches { get; set; } = new List<WatchEntity>();
    }
}

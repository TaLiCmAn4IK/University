using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WatchShop.Entities
{
    /// <summary>
    /// Сущность "Пользователь" (Code First).
    /// Связь: один пользователь → много заказов (one-to-many).
    /// </summary>
    [Table("Users")]
    public class UserEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = "";

        [Required]
        [MaxLength(100)]
        public string Password { get; set; } = "";

        [Required]
        [MaxLength(20)]
        public string Role { get; set; } = "Client";

        [MaxLength(100)]
        public string? Email { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        public ICollection<OrderEntity> Orders { get; set; } = new List<OrderEntity>();
    }
}

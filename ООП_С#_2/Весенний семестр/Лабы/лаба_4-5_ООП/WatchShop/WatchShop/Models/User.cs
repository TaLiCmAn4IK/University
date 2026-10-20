using System;

namespace WatchShop.Models
{
    // модель пользователя для аутентификации и разграничения ролей
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string Role { get; set; } = "Client";
        public string Email { get; set; } = "";
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
    }
}
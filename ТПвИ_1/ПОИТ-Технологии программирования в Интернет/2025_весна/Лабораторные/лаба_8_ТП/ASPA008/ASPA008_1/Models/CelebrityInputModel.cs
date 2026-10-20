using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ASPA008_1.Models
{
    /// <summary>
    /// Модель для ввода / редактирования знаменитости. Используется в формах (Add/Edit)
    /// и при передаче во второй шаг (подтверждение).
    /// </summary>
    public class CelebrityInputModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Полное имя обязательно")]
        [Display(Name = "Полное имя")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Выберите национальность")]
        [Display(Name = "Национальность")]
        public string Nationality { get; set; } = string.Empty;

        [Display(Name = "Фотография")]
        public IFormFile? PhotoFile { get; set; }

        public string? ReqPhotoPath { get; set; }
    }
}

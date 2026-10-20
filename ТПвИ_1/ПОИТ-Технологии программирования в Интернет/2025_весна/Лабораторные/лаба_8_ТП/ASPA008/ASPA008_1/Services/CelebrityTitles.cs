namespace ASPA008_1.Services
{
    /// <summary>
    /// Сервис с заголовками представлений (CelerbrityTitles из условия).
    /// </summary>
    public class CelebrityTitles
    {
        public string Index   { get; set; } = "Знаменитости";
        public string Details { get; set; } = "Информация о знаменитости";
        public string Add     { get; set; } = "Добавить знаменитость";
        public string Confirm { get; set; } = "Подтверждение";
        public string Edit    { get; set; } = "Редактирование знаменитости";
        public string Delete  { get; set; } = "Удаление знаменитости";
    }
}

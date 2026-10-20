namespace ASPA0011_1.Models;

public class ChannelInfo
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string State { get; set; } = "ACTIVE"; // ACTIVE | CLOSED
    public string Description { get; set; } = string.Empty;
}
// Так как наше приложение обменивается строго определенными JSON-сообщениями,
// нам понадобятся C#-классы для их десериализации.
// Создай в корне проекта папку Models и добавь туда следующие файлы
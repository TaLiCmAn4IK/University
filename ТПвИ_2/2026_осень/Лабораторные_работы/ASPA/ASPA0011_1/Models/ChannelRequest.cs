namespace ASPA0011_1.Models;

public class ChannelRequest
{
    public string Command { get; set; } = string.Empty; // new, close, open, del
    public Guid? Id { get; set; } // Для точечных команд по ID
    public string? Name { get; set; }
    public string? State { get; set; } // Для фильтрации при удалении или создании
    public string? Description { get; set; }
    public string? Reason { get; set; } // Причина закрытия
}
// Запрос на создание или изменение каналов для JSON-2, 3, 4, 5, 6, 7, 8
// Сделаем универсальный класс запроса к эндпоинтам /api/channels.
// В зависимости от операции некоторые поля будут оставаться пустыми.
namespace ASPA0011_1.Models;

public class QueueItem
{
    public Guid Id { get; set; } // Идентификатор канала
    public string Data { get; set; } = string.Empty; // JSON-строка данных
}

// Элемент очереди для JSON-11
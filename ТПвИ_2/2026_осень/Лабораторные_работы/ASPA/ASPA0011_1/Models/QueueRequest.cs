namespace ASPA0011_1.Models;

public class QueueRequest
{
    public string Command { get; set; } = string.Empty; // dequeue, peek, enqueue
    public Guid Id { get; set; } // Идентификатор канала
    public string? Data { get; set; } // Строка JSON (используется для enqueue)
}
// Запрос к очереди для JSON-9, 10
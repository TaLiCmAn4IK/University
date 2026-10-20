using Microsoft.AspNetCore.Mvc;
using ASPA0011_1.Models;
using ASPA0011_1.Services;

namespace ASPA0011_1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QueueController : ControllerBase
{
    private readonly ChannelService _channelService;
    private readonly ILogger<QueueController> _logger;

    public QueueController(ChannelService channelService, ILogger<QueueController> logger)
    {
        _channelService = channelService;
        _logger = logger;
        _logger.LogTrace(1007, "Создан экземпляр QueueController");
    }

    // POST /api/queue
    [HttpPost]
    public async Task<IActionResult> HandleQueueOperation([FromBody] QueueRequest request)
    {
        _logger.LogTrace(1008, "Вызов метода HandleQueueOperation (POST /api/queue) для канала {id}, команда {Command}", request.Id, request.Command);

        var (exists, isActive) = _channelService.GetChannelStatus(request.Id);
        if (!exists)
        {
            _logger.LogError(4044, "Операция очереди отклонена: канал {id} не существует", request.Id);
            return NotFound(); // Status 404
        }

        string cmd = request.Command.ToLower();

        // 1. Операция ENQUEUE (Поставить в очередь)
        if (cmd == "enqueue")
        {
            if (string.IsNullOrEmpty(request.Data))
            {
                return BadRequest("Поле 'data' не должно быть пустым для операции enqueue.");
            }

            try
            {
                bool success = await _channelService.EnqueueMessageAsync(request.Id, request.Data);
                if (!success) return BadRequest("Не удалось поставить сообщение в очередь.");

                _logger.LogDebug(1101, "Сообщение успешно добавлено в канал {id}", request.Id);

                return Ok(new QueueItem { Id = request.Id, Data = request.Data }); // Status 200
            }
            catch (TimeoutException ex)
            {
                _logger.LogWarning(3004, "Истекло время ожидания WaitEnqueue: {Message}", ex.Message);
                return StatusCode(408, ex.Message); // Request Timeout
            }
        }

        // 2. Операция DEQUEUE (Извлечь из очереди)
        if (cmd == "dequeue")
        {
            var data = await _channelService.DequeueMessageAsync(request.Id);
            if (data == null)
            {
                return NoContent(); // Очередь пуста
            }

            _logger.LogDebug(1102, "Сообщение извлечено (dequeue) из канала {id}", request.Id);
            return Ok(new QueueItem { Id = request.Id, Data = data });
        }

        // 3. Операция PEEK (Прочитать первый без извлечения)
        if (cmd == "peek")
        {
            var data = _channelService.PeekMessage(request.Id);
            if (data == null)
            {
                return NoContent(); // Очередь пуста
            }

            _logger.LogDebug(1103, "Выполнен просмотр (peek) первого сообщения в канале {id}", request.Id);
            return Ok(new QueueItem { Id = request.Id, Data = data });
        }

        return BadRequest("Неподдерживаемая команда очереди. Используйте 'enqueue', 'dequeue' или 'peek'.");
    }
}
// Теперь создадим второй контроллер,
// который будет отвечать за операции с самими сообщениями внутри каналов
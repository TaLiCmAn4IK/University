using Microsoft.AspNetCore.Mvc;
using ASPA0011_1.Models;
using ASPA0011_1.Services;

namespace ASPA0011_1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChannelsController : ControllerBase
{
    private readonly ChannelService _channelService;
    private readonly ILogger<ChannelsController> _logger;

    public ChannelsController(ChannelService channelService, ILogger<ChannelsController> logger)
    {
        _channelService = channelService;
        _logger = logger;
        // Лог уровня Trace при создании контроллера по заданию
        _logger.LogTrace(1001, "Создан экземпляр ChannelsController");
    }

    // GET /api/channels
    [HttpGet]
    public IActionResult GetChannels()
    {
        _logger.LogTrace(1002, "Вызов метода GetChannels (GET /api/channels)");
        var channels = _channelService.GetAllChannels();

        if (!channels.Any())
        {
            return NoContent(); // Status 204
        }
        return Ok(channels); // Status 200
    }

    // GET /api/channels/{guid}
    [HttpGet("{id:guid}")]
    public IActionResult GetChannel(Guid id)
    {
        _logger.LogTrace(1003, "Вызов метода GetChannel (GET /api/channels/{Id}) с параметром Id: {id}", id);
        var channel = _channelService.GetChannelById(id);

        if (channel == null)
        {
            _logger.LogError(4041, "Канал с ID {id} не найден (404 Not Found)", id);
            return NotFound(); // Status 404
        }
        return Ok(channel); // Status 200
    }

    // POST /api/channels
    [HttpPost]
    public IActionResult CreateChannel([FromBody] ChannelRequest request)
    {
        _logger.LogTrace(1004, "Вызов метода CreateChannel (POST /api/channels) с командой {Command}", request.Command);

        if (request.Command.ToLower() != "new")
        {
            _logger.LogWarning(3001, "Получена неверная команда создания канала: {Command}", request.Command);
            return BadRequest("Неверная команда. Ожидалось 'new'.");
        }

        var name = request.Name ?? "Без названия";
        var desc = request.Description ?? "";
        var state = request.State ?? "ACTIVE";

        var newChannel = _channelService.CreateChannel(name, desc, state);

        _logger.LogInformation(2001, "Создан новый канал. ID: {Id}, Имя: {Name}, Состояние: {State}", newChannel.Id, newChannel.Name, newChannel.State);

        if (newChannel.State == "CLOSED")
        {
            return StatusCode(204, newChannel); // Status 204 (CLOSED) по таблице задания
        }

        return StatusCode(201, newChannel); // Status 201 (ACTIVE)
    }

    // PUT /api/channels
    [HttpPut]
    public IActionResult UpdateChannels([FromBody] ChannelRequest request)
    {
        _logger.LogTrace(1005, "Вызов метода UpdateChannels (PUT /api/channels) с командой {Command}", request.Command);

        string cmd = request.Command.ToLower();

        // Изменить состояние ВСЕХ каналов
        if (request.Id == null)
        {
            if (cmd == "close")
            {
                _channelService.CloseAllChannels();
                _logger.LogInformation(2002, "Все каналы были принудительно ОСТАНОВЛЕНЫ (close)");
                return Ok(_channelService.GetAllChannels());
            }
            if (cmd == "open")
            {
                _channelService.OpenAllChannels();
                _logger.LogInformation(2003, "Все каналы были ВОЗОБНОВЛЕНЫ (open)");
                return Ok(_channelService.GetAllChannels());
            }
        }
        // Изменить состояние КОНКРЕТНОГО id-канала
        else
        {
            Guid id = request.Id.Value;
            var (exists, isActive) = _channelService.GetChannelStatus(id);

            if (!exists)
            {
                _logger.LogError(4042, "Попытка изменить состояние несуществующего канала {id}", id);
                return NotFound();
            }

            if (cmd == "close")
            {
                if (!isActive) _logger.LogWarning(3002, "Команда close вызвана для уже ЗАКРЫТОГО канала {id}", id);
                _channelService.CloseChannel(id);
                _logger.LogInformation(2004, "Канал {id} ОСТАНОВЛЕН", id);
                return Ok(_channelService.GetChannelById(id));
            }
            if (cmd == "open")
            {
                if (isActive) _logger.LogWarning(3003, "Команда open вызвана для уже АКТИВНОГО канала {id}", id);
                _channelService.OpenChannel(id);
                _logger.LogInformation(2005, "Канал {id} ВОЗОБНОВЛЕН", id);
                return Ok(_channelService.GetChannelById(id));
            }
        }

        return BadRequest("Неверная комбинация команды или параметров.");
    }

    // DELETE /api/channels
    [HttpDelete]
    public IActionResult DeleteChannels([FromBody] ChannelRequest request)
    {
        _logger.LogTrace(1006, "Вызов метода DeleteChannels (DELETE /api/channels) с командой {Command}", request.Command);

        if (request.Command.ToLower() != "del")
        {
            return BadRequest("Ожидалась команда 'del'.");
        }

        // Если передан конкретный ID (Задание на удаление канала)
        if (request.Id != null)
        {
            Guid id = request.Id.Value;
            var channelToDelete = _channelService.GetChannelById(id);
            if (channelToDelete == null)
            {
                _logger.LogError(4043, "Попытка удаления несуществующего канала {id}", id);
                return NotFound();
            }
            _channelService.DeleteChannel(id);
            _logger.LogInformation(2006, "Канал {id} успешно УДАЛЕН", id);
            return Ok(channelToDelete);
        }

        // Если ID не передан, смотрим на состояние (Удалить все или только CLOSED)
        if (request.State?.ToUpper() == "CLOSED")
        {
            _channelService.DeleteClosedChannels();
            _logger.LogInformation(2007, "Удалены все каналы со статусом CLOSED");
        }
        else
        {
            _channelService.DeleteAllChannels();
            _logger.LogInformation(2008, "Удалены абсолютно ВСЕ каналы");
        }

        return Ok(_channelService.GetAllChannels());
    }
}
// создать контроллеры, которые будут принимать HTTP-запросы из Postman и вызывать методы
// нашего ChannelService.По заданию эндпоинты должны возвращать JSON-ответы
// и строго определенные HTTP-статусы (200, 201, 204, 404)
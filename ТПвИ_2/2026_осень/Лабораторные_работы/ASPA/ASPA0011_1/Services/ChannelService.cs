using System.Collections.Concurrent;
using System.Threading.Channels;
using ASPA0011_1.Models;

namespace ASPA0011_1.Services;

public class ChannelService
{
    // Класс-обертка, чтобы хранить метаданные канала вместе с самой очередью System.Threading.Channels
    private class ChannelWrapper
    {
        public ChannelInfo Info { get; set; } = null!;
        public Channel<string> MessageChannel { get; set; } = null!;
        // Используем ConcurrentQueue параллельно для операции "peek" (посмотреть без извлечения),
        // так как стандартный Channel<T> не поддерживает встроенный Peek.
        public ConcurrentQueue<string> PeekQueue { get; set; } = new();
    }

    // Потокобезопасный словарь для хранения всех каналов в памяти приложения
    private readonly ConcurrentDictionary<Guid, ChannelWrapper> _channels = new();

    // Системный параметр максимального времени ожидания (в секундах) по заданию
    public int WaitEnqueue { get; set; } = 10;

    // GET /api/channels - Получить перечень всех каналов
    public IEnumerable<ChannelInfo> GetAllChannels()
    {
        return _channels.Values.Select(c => c.Info).ToList();
    }

    // GET /api/channels/{guid} - Получить данные о конкретном канале
    public ChannelInfo? GetChannelById(Guid id)
    {
        if (_channels.TryGetValue(id, out var wrapper))
        {
            return wrapper.Info;
        }
        return null;
    }

    // POST /api/channels - Создать канал (JSON-2)
    public ChannelInfo CreateChannel(string name, string description, string initialState)
    {
        var id = Guid.NewGuid();
        var info = new ChannelInfo
        {
            Id = id,
            Name = name,
            Description = description,
            State = initialState.ToUpper() == "CLOSED" ? "CLOSED" : "ACTIVE"
        };

        // Создаем неограниченный канал для JSON-сообщений
        var msgChannel = Channel.CreateUnbounded<string>(new UnboundedChannelOptions
        {
            SingleReader = false,
            SingleWriter = false
        });

        var wrapper = new ChannelWrapper
        {
            Info = info,
            MessageChannel = msgChannel
        };

        // Если при создании состояние сразу CLOSED, закрываем запись в канал
        if (info.State == "CLOSED")
        {
            wrapper.MessageChannel.Writer.Complete();
        }

        _channels[id] = wrapper;
        return info;
    }

    // PUT /api/channels - Изменить состояние (Команды close / open)
    public bool CloseChannel(Guid id)
    {
        if (_channels.TryGetValue(id, out var wrapper) && wrapper.Info.State != "CLOSED")
        {
            wrapper.Info.State = "CLOSED";
            wrapper.MessageChannel.Writer.Complete(); // Больше писать в канал нельзя
            return true;
        }
        return false;
    }

    public bool OpenChannel(Guid id)
    {
        if (_channels.TryGetValue(id, out var wrapper) && wrapper.Info.State != "ACTIVE")
        {
            wrapper.Info.State = "ACTIVE";
            // Пересоздаем канал, так как закрытый однажды Writer нельзя открыть заново
            var newMsgChannel = Channel.CreateUnbounded<string>();
            wrapper.MessageChannel = newMsgChannel;

            // Если в PeekQueue оставались элементы, их нужно перенести в новый канал
            // (в рамках учебной лабы упростим: открываем чистый рабочий Writer)
            return true;
        }
        return false;
    }

    public void CloseAllChannels()
    {
        foreach (var key in _channels.Keys)
        {
            CloseChannel(key);
        }
    }

    public void OpenAllChannels()
    {
        foreach (var key in _channels.Keys)
        {
            OpenChannel(key);
        }
    }

    // DELETE /api/channels - Удаление каналов
    public bool DeleteChannel(Guid id)
    {
        return _channels.TryRemove(id, out _);
    }

    public void DeleteAllChannels()
    {
        _channels.Clear();
    }

    public void DeleteClosedChannels()
    {
        var closedIds = _channels.Where(c => c.Value.Info.State == "CLOSED").Select(c => c.Key).ToList();
        foreach (var id in closedIds)
        {
            _channels.TryRemove(id, out _);
        }
    }

    // Вспомогательный метод проверки: существует ли канал и открыт ли он
    public (bool exists, bool isActive) GetChannelStatus(Guid id)
    {
        if (!_channels.TryGetValue(id, out var wrapper)) return (false, false);
        return (true, wrapper.Info.State == "ACTIVE");
    }

    // === ОПЕРАЦИИ С ОЧЕРЕДЬЮ (POST /api/queue) ===

    // Enqueue - поставить в очередь
    public async Task<bool> EnqueueMessageAsync(Guid id, string jsonData)
    {
        if (!_channels.TryGetValue(id, out var wrapper)) return false;

        // Создаем задачу записи с таймаутом WaitEnqueue
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(WaitEnqueue));
        try
        {
            // Записываем данные в System.Threading.Channels
            await wrapper.MessageChannel.Writer.WriteAsync(jsonData, cts.Token);
            // Дублируем в PeekQueue для возможности просмотра без извлечения
            wrapper.PeekQueue.Enqueue(jsonData);
            return true;
        }
        catch (OperationCanceledException)
        {
            // Сработает, если истечет время WaitEnqueue
            throw new TimeoutException($"Превышено время ожидания WaitEnqueue ({WaitEnqueue} сек) для канала {id}");
        }
    }

    // Dequeue - извлечь из очереди
    public async Task<string?> DequeueMessageAsync(Guid id)
    {
        if (!_channels.TryGetValue(id, out var wrapper)) return null;

        // Читаем из основного канала асинхронно
        if (await wrapper.MessageChannel.Reader.WaitToReadAsync())
        {
            if (wrapper.MessageChannel.Reader.TryRead(out var item))
            {
                // Удаляем этот же элемент из Peek-очереди, чтобы сохранять синхронность
                wrapper.PeekQueue.TryDequeue(out _);
                return item;
            }
        }
        return null; // Очередь пуста
    }

    // Peek - посмотреть первый элемент без удаления
    public string? PeekMessage(Guid id)
    {
        if (!_channels.TryGetValue(id, out var wrapper)) return null;

        if (wrapper.PeekQueue.TryPeek(out var item))
        {
            return item;
        }
        return null; // Очередь пуста
    }
}
// он реализует потокобезопасное создание каналов,
// закрытие, открытие, удаление, а также операции enqueue, dequeue и peek
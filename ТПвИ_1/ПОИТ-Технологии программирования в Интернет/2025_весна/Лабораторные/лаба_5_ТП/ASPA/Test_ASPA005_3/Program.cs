using System.Text.Json;

string baseUrl = "https://localhost:7048"; // Укажите ваш порт

using HttpClient client = new HttpClient();

Console.WriteLine("=== Тестирование ASPA005_3 ===\n");

// Группа A
Console.WriteLine("--- Группа A ---");

// GET /A/x (x <= 100)
await TestEndpoint(client, $"{baseUrl}/A/50", "GET", 200);
await TestEndpoint(client, $"{baseUrl}/A/150", "GET", 404);

// POST /A/x (0 <= x <= 100)
await TestEndpoint(client, $"{baseUrl}/A/75", "POST", 200);
await TestEndpoint(client, $"{baseUrl}/A/150", "POST", 404);
await TestEndpoint(client, $"{baseUrl}/A/-5", "POST", 404);

// PUT /A/x/y (x >= 1, y >= 1)
await TestEndpoint(client, $"{baseUrl}/A/5/10", "PUT", 200);
await TestEndpoint(client, $"{baseUrl}/A/0/10", "PUT", 404);
await TestEndpoint(client, $"{baseUrl}/A/5/0", "PUT", 404);

// DELETE /A/x-y (x >= 1, 1 <= y <= 100)
await TestEndpoint(client, $"{baseUrl}/A/5-10", "DELETE", 200);
await TestEndpoint(client, $"{baseUrl}/A/0-10", "DELETE", 404);
await TestEndpoint(client, $"{baseUrl}/A/5-150", "DELETE", 404);
await TestEndpoint(client, $"{baseUrl}/A/5-0", "DELETE", 404);

Console.WriteLine();

// Группа B
Console.WriteLine("--- Группа B ---");

// GET /B/x
await TestEndpoint(client, $"{baseUrl}/B/3.14", "GET", 200);
await TestEndpoint(client, $"{baseUrl}/B/abc", "GET", 404);

// POST /B/x/y
await TestEndpoint(client, $"{baseUrl}/B/1.5/2.5", "POST", 200);
await TestEndpoint(client, $"{baseUrl}/B/1.5/abc", "POST", 404);

// DELETE /B/x-y
await TestEndpoint(client, $"{baseUrl}/B/1.5-2.5", "DELETE", 200);
await TestEndpoint(client, $"{baseUrl}/B/1.5-abc", "DELETE", 404);

Console.WriteLine();

// Группа C
Console.WriteLine("--- Группа C ---");

// GET /C/x
await TestEndpoint(client, $"{baseUrl}/C/true", "GET", 200);
await TestEndpoint(client, $"{baseUrl}/C/false", "GET", 200);
await TestEndpoint(client, $"{baseUrl}/C/yes", "GET", 404);

// POST /C/x,y
await TestEndpoint(client, $"{baseUrl}/C/true,false", "POST", 200);
await TestEndpoint(client, $"{baseUrl}/C/true,true", "POST", 200);
await TestEndpoint(client, $"{baseUrl}/C/true,abc", "POST", 404);

Console.WriteLine();

// Группа D
Console.WriteLine("--- Группа D ---");

// GET /D/x
await TestEndpoint(client, $"{baseUrl}/D/2024-12-25", "GET", 200);
await TestEndpoint(client, $"{baseUrl}/D/25.12.2024", "GET", 200);
await TestEndpoint(client, $"{baseUrl}/D/abc", "GET", 404);

// POST /D/x|y
await TestEndpoint(client, $"{baseUrl}/D/2024-12-25|2024-12-26", "POST", 200);
await TestEndpoint(client, $"{baseUrl}/D/abc|2024-12-26", "POST", 404);

Console.WriteLine();

// Группа E
Console.WriteLine("--- Группа E ---");

// GET /E/12-x
await TestEndpoint(client, $"{baseUrl}/E/12-Hello", "GET", 200);
await TestEndpoint(client, $"{baseUrl}/E/12-", "GET", 404);

// PUT /E/x (только буквы, длина 2-12)
await TestEndpoint(client, $"{baseUrl}/E/Hello", "PUT", 200);
await TestEndpoint(client, $"{baseUrl}/E/H", "PUT", 404);
await TestEndpoint(client, $"{baseUrl}/E/Hello123", "PUT", 404);
await TestEndpoint(client, $"{baseUrl}/E/HelloWorldHello", "PUT", 404);

Console.WriteLine();

// Группа F 
Console.WriteLine("--- Группа F ---");

// PUT /F/x (email, домен .by)
await TestEndpoint(client, $"{baseUrl}/F/user@domain.by", "PUT", 200);
await TestEndpoint(client, $"{baseUrl}/F/user@gmail.com", "PUT", 404);
await TestEndpoint(client, $"{baseUrl}/F/user@.by", "PUT", 404);
await TestEndpoint(client, $"{baseUrl}/F/userdomain.by", "PUT", 404);

Console.WriteLine("\n=== Тестирование завершено ===");

// Вспомогательный метод 
async Task TestEndpoint(HttpClient client, string url, string method, int expectedStatus)
{
    try
    {
        HttpResponseMessage response;

        switch (method.ToUpper())
        {
            case "GET":
                response = await client.GetAsync(url);
                break;
            case "POST":
                response = await client.PostAsync(url, null);
                break;
            case "PUT":
                response = await client.PutAsync(url, null);
                break;
            case "DELETE":
                response = await client.DeleteAsync(url);
                break;
            default:
                response = await client.GetAsync(url);
                break;
        }

        int statusCode = (int)response.StatusCode;
        string status = statusCode == expectedStatus ? "OK" : "NO";

        Console.WriteLine($"{status} {method} {url} -> {statusCode} (ожидалось {expectedStatus})");

        if (statusCode == 200)
        {
            var body = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"   Ответ: {body}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка: {ex.Message}");
    }
}
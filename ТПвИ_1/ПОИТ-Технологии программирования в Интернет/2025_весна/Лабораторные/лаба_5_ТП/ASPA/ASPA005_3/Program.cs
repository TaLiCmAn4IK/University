var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseHttpsRedirection();

// GET /A/x - x целое число <= 100
app.MapGet("/A/{x:int:max(100)}", (int x) =>
    Results.Ok(new { route = "A", method = "GET", x, y = (int?)null }));

// POST /A/x - x целое число 0 <= x <= 100
app.MapPost("/A/{x:int:range(0,100)}", (int x) =>
    Results.Ok(new { route = "A", method = "POST", x, y = (int?)null }));

// PUT /A/x/y - x >= 1, y >= 1
app.MapPut("/A/{x:int:min(1)}/{y:int:min(1)}", (int x, int y) =>
    Results.Ok(new { route = "A", method = "PUT", x, y }));

// DELETE /A/x-y - x >= 1, y >= 1, y <= 100
app.MapDelete("/A/{x:int:min(1)}-{y:int:range(1,100)}", (int x, int y) =>
    Results.Ok(new { route = "A", method = "DELETE", x, y }));

// GET /B/x - x float
app.MapGet("/B/{x:float}", (float x) =>
    Results.Ok(new { route = "B", method = "GET", x, y = (float?)null }));

// POST /B/x/y - x float, y float
app.MapPost("/B/{x:float}/{y:float}", (float x, float y) =>
    Results.Ok(new { route = "B", method = "POST", x, y }));

// DELETE /B/x-y - x float, y float
app.MapDelete("/B/{x:float}-{y:float}", (float x, float y) =>
    Results.Ok(new { route = "B", method = "DELETE", x, y }));

// GET /C/x - x bool
app.MapGet("/C/{x:bool}", (bool x) =>
    Results.Ok(new { route = "C", method = "GET", x, y = (bool?)null }));

// POST /C/x,y - x bool, y bool
app.MapPost("/C/{x:bool},{y:bool}", (bool x, bool y) =>
    Results.Ok(new { route = "C", method = "POST", x, y }));

// GET /D/x - x DateTime
app.MapGet("/D/{x:datetime}", (DateTime x) =>
    Results.Ok(new { route = "D", method = "GET", x, y = (DateTime?)null }));

// POST /D/x|y - x DateTime, y DateTime
app.MapPost("/D/{x:datetime}|{y:datetime}", (DateTime x, DateTime y) =>
    Results.Ok(new { route = "D", method = "POST", x, y }));

// GET /E/12-x - x строка
app.MapGet("/E/12-{x:required}", (string x) =>
    Results.Ok(new { route = "E", method = "GET", x, y = (string?)null }));

// PUT /E/x - x строка, только буквы, длина 2-12
app.MapPut("/E/{x:alpha:length(2,12)}", (string x) =>
    Results.Ok(new { route = "E", method = "PUT", x, y = (string?)null }));

// PUT /F/x - x email, домен = by
app.MapPut("/F/{x:regex(^[^@]+@[^@]+\\.by$)}", (string x) =>
    Results.Ok(new { route = "F", method = "PUT", x, y = (string?)null }));

// Fallback для неподходящих маршрутов
app.MapFallback((HttpContext ctx) =>
    Results.NotFound(new { error = $"path {ctx.Request.Path} not supported" }));

app.Run();
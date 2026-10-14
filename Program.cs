var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapGet("/tasks", () => new[]
{
    new { id = 1, title = "Написать Dockerfile", done = false },
    new { id = 2, title = "Собрать образ", done = false }
});

app.Run();
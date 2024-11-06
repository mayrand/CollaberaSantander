using BestOfHackerNews.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<HttpClient>();
builder.Services.AddScoped<HackerService>();
builder.Services.AddMemoryCache();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/topstories", async (int n, HackerService hackerNewsService) =>
    {
        if (n <= 0)
        {
            return Results.BadRequest("The number of stories must be greater than zero.");
        }

        var stories = await hackerNewsService.GetTopStoriesAsync(n);
        return Results.Ok(stories);
    })
    .WithName("topstories")
    .WithOpenApi();

app.Run();
using kreddit_api.Data;
using kreddit_api.Service;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// CORS, så hjemmesiden må kalde API'et
var AllowSomeStuff = "_AllowSomeStuff";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowSomeStuff, policy => {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

// Database
builder.Services.AddDbContext<KredditContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("ContextSQLite")));

// DataService kan bruges i endpoints via dependency injection
builder.Services.AddScoped<DataService>();

// Undgå fejl ved JSON-objekter der refererer til hinanden
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler =
        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

// Seed data hvis databasen er tom
using (var scope = app.Services.CreateScope())
{
    var dataService = scope.ServiceProvider.GetRequiredService<DataService>();
    dataService.SeedData();
}

app.UseHttpsRedirection();
app.UseCors(AllowSomeStuff);

app.MapGet("/", () => new { message = "Hello World!" });

app.MapGet("/api/posts", (DataService service) => service.GetPosts());

app.MapGet("/api/posts/{id}", (DataService service, int id) => service.GetPost(id));

app.MapPost("/api/posts/{id}/comments", (DataService service, int id, NewCommentData data) =>
{
    var comment = service.CreateComment(id, data.Content, data.Author);
    return comment is null ? Results.NotFound() : Results.Ok(comment);
});

app.MapPost("/api/posts", (DataService service, NewPostData data) =>
{
    if (string.IsNullOrWhiteSpace(data.Title) || string.IsNullOrWhiteSpace(data.Author))
        return Results.BadRequest("Title and author are required");

    var post = service.CreatePost(data.Title, data.Content ?? "", data.Url, data.Author);
    return Results.Ok(post);
});
app.Run();

record NewCommentData(string Content, string Author);
record NewPostData(string Title, string? Content, string? Url, string Author);
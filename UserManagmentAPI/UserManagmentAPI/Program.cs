using System.Text.RegularExpressions;
using UserManagementAPI.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


// ----------------- MIDDLEWARE -----------------

// Error handling middleware (always first)
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Unhandled exception");
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { error = "Internal server error." });
    }
});

// Authentication middleware
app.Use(async (context, next) =>
{
    var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");

    if (string.IsNullOrEmpty(token) || token != "mysecrettoken")
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsJsonAsync(new { error = "Unauthorized" });
        return;
    }

    await next();
});

// Logging middleware (last)
app.Use(async (context, next) =>
{
    app.Logger.LogInformation("Incoming Request: {method} {path}", context.Request.Method, context.Request.Path);

    await next();

    app.Logger.LogInformation("Outgoing Response: {statusCode}", context.Response.StatusCode);
});


// ----------------- USER API -----------------

var users = new List<User>();
var nextId = 1;

bool IsValidEmail(string email) =>
    Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

// GET all users
app.MapGet("/api/users", () =>
{
    return Results.Ok(users);
});

// GET user by id
app.MapGet("/api/users/{id:int}", (int id) =>
{
    var user = users.FirstOrDefault(u => u.Id == id);
    return user is not null
        ? Results.Ok(user)
        : Results.NotFound(new { Message = $"User with ID {id} not found." });
});

// POST create user
app.MapPost("/api/users", (User user) =>
{
    if (string.IsNullOrWhiteSpace(user.FirstName) ||
        string.IsNullOrWhiteSpace(user.LastName) ||
        !IsValidEmail(user.Email))
    {
        return Results.BadRequest(new { Message = "Invalid user data. Provide first name, last name, and valid email." });
    }

    user.Id = nextId++;
    users.Add(user);
    return Results.Created($"/api/users/{user.Id}", user);
});

// PUT update user
app.MapPut("/api/users/{id:int}", (int id, User updatedUser) =>
{
    var user = users.FirstOrDefault(u => u.Id == id);
    if (user is null) return Results.NotFound(new { Message = $"User with ID {id} not found." });

    if (string.IsNullOrWhiteSpace(updatedUser.FirstName) ||
        string.IsNullOrWhiteSpace(updatedUser.LastName) ||
        !IsValidEmail(updatedUser.Email))
    {
        return Results.BadRequest(new { Message = "Invalid user data. Provide first name, last name, and valid email." });
    }

    user.FirstName = updatedUser.FirstName;
    user.LastName = updatedUser.LastName;
    user.Email = updatedUser.Email;
    user.Department = updatedUser.Department;

    return Results.Ok(user);
});

// DELETE user
app.MapDelete("/api/users/{id:int}", (int id) =>
{
    var user = users.FirstOrDefault(u => u.Id == id);
    if (user is null) return Results.NotFound(new { Message = $"User with ID {id} not found." });

    users.Remove(user);
    return Results.NoContent();
});

app.Run();


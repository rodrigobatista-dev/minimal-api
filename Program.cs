var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", static (MinimalApi.DTOs.LoginDTO loginDTO) =>
{
    if (loginDTO.Email == "admin@teste.com" && loginDTO.Password == "1234")
    {
        return Results.Ok("Login successful");
    }
    return Results.Unauthorized();
});

app.Run();

public class LoginDTO
{
    public string Email { get; set; }
    public string Password { get; set; }
}
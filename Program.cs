using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.Entity;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.Dominio.Service;
using MinimalApi.DTOs;
using MinimalApi.Infrastructure.Db;

#region Builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DbContexto>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
            ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))));

var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home()));
#endregion

#region Administradores
app.MapPost("/administradores/login", static ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) =>
{
    if (administradorServico.Login(loginDTO) != null)
    {
        return Results.Ok("Login successful");
    }
    return Results.Unauthorized();
});
#endregion

#region Veiculos
app.MapPost("/veiculos", static ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
{
    var veiculo = new Veiculo
    {
        Nome = veiculoDTO.Nome ?? string.Empty,
        Modelo = veiculoDTO.Modelo ?? string.Empty,
        Ano = veiculoDTO.Ano
    };
    veiculoServico.Incluir(veiculo);

    return Results.Created($"/veiculos/{veiculo.Id}", veiculo);
});


#endregion


#region app
app.UseSwagger();
app.UseSwaggerUI();
app.Run();
#endregion

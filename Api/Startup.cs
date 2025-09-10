using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using minimal_api.Dominio.ModelViews;
using MinimalApi;
using MinimalApi.Dominio.Entity;
using MinimalApi.Dominio.Enuns;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.Dominio.Service;
using MinimalApi.DTOs;
using MinimalApi.Infrastructure.Db;


public class Startup
{
    public Startup(IConfiguration configuration)
    {

        Configuration = configuration;
        key = Configuration?.GetSection("Jwt")?.ToString() ?? "";
    }

    private string key = "";
    public IConfiguration Configuration { get; set; } = default!;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>
{
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});
        services.AddAuthorization();

        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        services.AddScoped<IAdministradorServico, AdministradorServico>();
        services.AddScoped<IVeiculoServico, VeiculoServico>();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Insira o token JWT aqui"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
            });
        });

        services.AddDbContext<DbContexto>(options =>
        {
            options.UseMySql(
                Configuration.GetConnectionString("MySql"),
                    ServerVersion.AutoDetect(Configuration.GetConnectionString("MySql"))

                );
        });

    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseRouting();   

        app.UseAuthentication();
        app.UseAuthorization();


        app.UseEndpoints(endpoint =>
        {
            #region Home
            endpoint.MapGet("/", () => Results.Json(new Home())).WithTags("Home");

            #endregion

            #region Administradores
            string GerarToken(Administrador administrador)
            {
                if (string.IsNullOrEmpty(key)) return string.Empty;

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>()
    {
        new Claim("Email", administrador.Email),
        new Claim("Perfil", administrador.Perfil),
        new Claim(ClaimTypes.Role, administrador.Perfil),
    };

                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: credentials
                );
                return new JwtSecurityTokenHandler().WriteToken(token);
            }

            ErrosDeValidacao validaADMDTO(AdministradoresDTO administradoresDTO)
            {
                var validacao = new ErrosDeValidacao();

                if (string.IsNullOrEmpty(administradoresDTO.Email))
                {
                    validacao.Mensagens.Add("O email é obrigatória.");
                }
                if (administradoresDTO.Senha == null || administradoresDTO.Senha.Length < 6)
                {
                    validacao.Mensagens.Add("A senha  é inválida. Deve conter ao menos 6 caracteres.");
                }
                if (!Enum.TryParse<Perfil>(administradoresDTO.Perfil, true, out var perfilValido))
                {
                    validacao.Mensagens.Add("O perfil do administrador é inválido. Deve ser 'Comum' ou 'Admin'.");

                }
                return validacao;
            }



            endpoint.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) =>
            {
                var adm = administradorServico.Login(loginDTO);
                if (adm != null)
                {
                    string token = GerarToken(adm);

                    return Results.Ok(new AdministradorLogado
                    {
                        Email = adm.Email,
                        Perfil = adm.Perfil,
                        Token = token
                    });
                }
                return Results.Unauthorized();
            }).WithTags("Administradores");

            endpoint.MapPost("/administradores", ([FromBody] AdministradoresDTO administradoresDTO, IAdministradorServico administradorServico) =>
            {
                var validacao = validaADMDTO(administradoresDTO);
                if (validacao.Mensagens.Count > 0)
                {
                    return Results.BadRequest(validacao);
                }

                var administrador = new Administrador
                {
                    Email = administradoresDTO.Email ?? string.Empty,
                    Senha = administradoresDTO.Senha ?? string.Empty,
                    Perfil = administradoresDTO.Perfil != null ? administradoresDTO.Perfil.ToString() : Perfil.Comum.ToString()
                };
                administradorServico.Incluir(administrador);

                return Results.Created($"/administradores/{administrador.Id}", new AdministradorModeViews
                {
                    Id = administrador.Id,
                    Email = administrador.Email,
                    Perfil = administrador.Perfil
                });
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" })
            .WithTags("Administradores");

            endpoint.MapGet("/administradores", ([FromQuery] int? pagina, IAdministradorServico administradorServico) =>
            {

                var adms = new List<AdministradorModeViews>();
                var administradores = administradorServico.Todos(pagina);
                foreach (var adm in administradores)
                {
                    adms.Add(new AdministradorModeViews
                    {
                        Id = adm.Id,
                        Email = adm.Email,
                        Perfil = adm.Perfil
                    });
                }
                return Results.Ok(adms);

            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" })
            .WithTags("Administradores");

            endpoint.MapGet("administradores/{id}", ([FromRoute] int id, IAdministradorServico administradorServico) =>
            {
                var administrador = administradorServico.BuscarPorId(id);
                if (administrador == null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(new AdministradorModeViews
                {
                    Id = administrador.Id,
                    Email = administrador.Email,
                    Perfil = administrador.Perfil
                });
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" })
            .WithTags("Administradores");

            endpoint.MapPut("/administradores/{id}", ([FromRoute] int id, [FromBody] AdministradoresDTO administradoresDTO, IAdministradorServico administradorServico) =>
            {
                var administrador = administradorServico.BuscarPorId(id);
                if (administrador == null)
                {
                    return Results.NotFound();
                }
                var validacao = validaADMDTO(administradoresDTO);
                if (validacao.Mensagens.Count > 0)
                {
                    return Results.BadRequest(validacao);
                }
                administrador.Email = administradoresDTO.Email ?? administrador.Email;
                administrador.Senha = administradoresDTO.Senha ?? administrador.Senha;
                administrador.Perfil = administradoresDTO.Perfil.ToString();

                administradorServico.Atualizar(administrador);
                return Results.Ok(administrador);

            }).RequireAuthorization().WithTags("Administradores");

            endpoint.MapDelete("/administradores/{id}", ([FromRoute] int id, IAdministradorServico administradorServico) =>
            {
                var administrador = administradorServico.BuscarPorId(id);
                if (administrador == null)
                {
                    return Results.NotFound();
                }

                administradorServico.Apagar(administrador);
                return Results.Ok();
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" })
            .WithTags("Administradores");

            #endregion

            #region Veiculos
            ErrosDeValidacao validaDTO(VeiculoDTO veiculoDTO)
            {
                var validacao = new ErrosDeValidacao();

                if (string.IsNullOrEmpty(veiculoDTO.Nome))
                {
                    validacao.Mensagens.Add("O nome do veículo é obrigatório.");
                }
                if (string.IsNullOrEmpty(veiculoDTO.Marca))
                {
                    validacao.Mensagens.Add("A marca do veículo é obrigatória.");
                }
                if (veiculoDTO.Ano < 1886 || veiculoDTO.Ano > DateTime.Now.Year + 1)
                {
                    validacao.Mensagens.Add("O ano do veículo é inválido.");
                }
                return validacao;
            }

            endpoint.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
            {

                var validacao = validaDTO(veiculoDTO);
                if (validacao.Mensagens.Count > 0)
                {
                    return Results.BadRequest(validacao);
                }

                var veiculo = new Veiculo
                {
                    Nome = veiculoDTO.Nome ?? string.Empty,
                    Marca = veiculoDTO.Marca ?? string.Empty,
                    Ano = veiculoDTO.Ano
                };
                veiculoServico.Incluir(veiculo);

                return Results.Created($"/veiculos/{veiculo.Id}", veiculo);
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin, Comum" })
            .WithTags("Veiculos");

            endpoint.MapGet("/veiculos", ([FromQuery] int? pagina, IVeiculoServico veiculoServico) =>
            {

                var veiculos = veiculoServico.Todos(pagina);
                return Results.Ok(veiculos);
            }).RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin, Comum" })
            .WithTags("Veiculos");

            endpoint.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
            {
                var veiculo = veiculoServico.BuscarPorId(id);
                if (veiculo == null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(veiculo);
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin, Comum" })
            .WithTags("Veiculos");

            endpoint.MapPut("/veiculos/{id}", ([FromRoute] int id, [FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
            {
                var veiculo = veiculoServico.BuscarPorId(id);
                if (veiculo == null)
                {
                    return Results.NotFound();
                }

                var validacao = validaDTO(veiculoDTO);
                if (validacao.Mensagens.Count > 0)
                {
                    return Results.BadRequest(validacao);
                }



                veiculo.Nome = veiculoDTO.Nome ?? veiculo.Nome;
                veiculo.Marca = veiculoDTO.Marca ?? veiculo.Marca;
                veiculo.Ano = veiculoDTO.Ano != 0 ? veiculoDTO.Ano : veiculo.Ano;

                veiculoServico.Atualizar(veiculo);
                return Results.Ok(veiculo);
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" })
            .WithTags("Veiculos");

            endpoint.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
            {
                var veiculo = veiculoServico.BuscarPorId(id);
                if (veiculo == null)
                {
                    return Results.NotFound();
                }

                veiculoServico.Apagar(veiculo);
                return Results.Ok();
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" })
            .WithTags("Veiculos");

            #endregion

        });

    }

}
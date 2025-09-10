
using MinimalApi.Dominio.Entity;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.DTOs;

namespace Test.Mocks;

public class AdministradorServicoMock : IAdministradorServico
{
    private static List<Administrador> administradores = new List<Administrador>()
    {
        new Administrador{
            Id = 1,
            Email = "adm@teste.com",
            Senha = "123456",
            Perfil = "Adm"
        },
        new Administrador{
            Id = 2,
            Email = "editor@teste.com",
            Senha = "123456",
            Perfil = "Editor"
        }
    };
    public Administrador? BuscarPorId(int id)
    {
        return administradores.Find(a => a.Id == id);
    }
    public void Incluir(Administrador administrador)
    {
        administrador.Id = administradores.Count() + 1;
        administradores.Add(administrador);
               

    }
    public List<Administrador> Todos(int? pagina = 1, string? email = null)
    {
        return administradores;
    }
    public Administrador? Login(LoginDTO loginDTO)
    {
        return administradores.Find(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Password);
    }
    public void Apagar(Administrador administrador)
    {
        administradores.Remove(administrador);
    }

    public void Atualizar(Administrador administrador)
    {
        var adms = administradores.FindIndex(a => a.Id == administrador.Id);
        if (adms != -1)
        {
            administradores[adms] = administrador;
        }
    }
}





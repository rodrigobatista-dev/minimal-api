using MinimalApi.Dominio.Entity;
using MinimalApi.DTOs;

namespace MinimalApi.Dominio.Interfaces;

public interface IAdministradorServico
{
   Administrador? Login(LoginDTO loginDTO);

   List<Administrador> Todos(int? pagina = 1, string? email = null);
   Administrador? BuscarPorId(int id);
   void Incluir(Administrador administrador);
   void Atualizar(Administrador administrador);
   void Apagar(Administrador administrador);
}

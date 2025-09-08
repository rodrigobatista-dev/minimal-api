using MinimalApi.Dominio.Entity;
using MinimalApi.DTOs;

namespace MinimalApi.Dominio.Interfaces;

public interface IAdministradorServico
{
   Administrador? Login(LoginDTO loginDTO);
}

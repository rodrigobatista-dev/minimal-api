using System.Data.Common;
using MinimalApi.Dominio.Entity;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.DTOs;
using MinimalApi.Infrastructure.Db;

namespace MinimalApi.Dominio.Service;

public class AdministradorServico : IAdministradorServico
{
    private readonly DbContexto _contexto;
    public AdministradorServico(DbContexto contexto)
    {
        _contexto = contexto;
    }

    public void Apagar(Administrador administrador)
    {
        _contexto.Administradores.Remove(administrador);
        _contexto.SaveChanges();
    }

    public void Atualizar(Administrador administrador)
    {
        _contexto.Administradores.Update(administrador);
        _contexto.SaveChanges();
    }

    public Administrador? BuscarPorId(int id)
    {
        return _contexto.Administradores.Find(id);
    }

    public void Incluir(Administrador administrador)
    {
        _contexto.Administradores.Add(administrador);
        _contexto.SaveChanges();
    }

    public Administrador? Login(LoginDTO loginDTO)
    {
        var adm = _contexto.Administradores.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Password).FirstOrDefault();
        return adm;
    }

    public List<Administrador> Todos(int? pagina = 1, string? email = null)
    {
        var query = _contexto.Administradores.AsQueryable();
        
        int itensPorPagina = 10;
        if(pagina != null)
            query = query.Skip(((pagina ?? 1) - 1) * itensPorPagina).Take(itensPorPagina);

        return query.ToList();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinimalApi.Dominio.Enuns;

namespace MinimalApi.DTOs
{
    public class AdministradoresDTO
    {
        public string? Email { get; set; }
        public string? Senha { get; set; }
        public string? Perfil { get; set; } 
    }
}
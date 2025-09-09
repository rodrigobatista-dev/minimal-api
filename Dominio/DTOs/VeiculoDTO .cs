using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MinimalApi.DTOs
{
    public class VeiculoDTO
    {
        public string? Nome { get; set; }
        public string? Modelo { get; set; }
        public int Ano { get; set; }
    }
}
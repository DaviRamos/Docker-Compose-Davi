using System;
using System.Collections.Generic;
using NetRedisASide.Data;
using NetRedisASide.Models;

namespace NetRedisASide.Data
{
    public class DataSeeder
    {
        private readonly AppDbContext _context;

        public DataSeeder(AppDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            // Seed a list of assuntos if the table is empty
            if (!_context.Assuntos.Any())
            {
                var assuntos = new List<Assunto>
                {
                    new Assunto { Nome = "Matemática", Descricao = "Estudo dos números e suas operações." },
                    new Assunto { Nome = "História", Descricao = "Estudo dos eventos passados." },
                    new Assunto { Nome = "Ciência", Descricao = "Estudo do mundo natural." },
                };
                var movimentacoes = new List<Movimentacao>
                {
                    new Movimentacao { Nome = "Entrada", Descricao = "Relatório mensal de vendas." },
                    new Movimentacao { Nome = "Saída", Descricao = "Fatura de serviços prestados." },
                };
                var tiposDocumento = new List<TipoDocumento>
                {
                    new TipoDocumento { Nome = "Relatório", Descricao = "Relatório mensal de vendas." },
                    new TipoDocumento { Nome = "Fatura", Descricao = "Fatura de serviços prestados." },
                    new TipoDocumento { Nome = "Orçamento", Descricao = "Orçamento para o próximo projeto." },
                };

                await _context.Assuntos.AddRangeAsync(assuntos);
                await _context.Movimentacoes.AddRangeAsync(movimentacoes);
                await _context.TiposDocumentos.AddRangeAsync(tiposDocumento);
                await _context.SaveChangesAsync();
            }
        }
    }
}

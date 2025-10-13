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
                    new Assunto { Nome = "Matemática", Descricao = "Estudo dos números e suas operações.", DataCriacao = DateTime.Now },
                    new Assunto { Nome = "História", Descricao = "Estudo dos eventos passados.", DataCriacao = DateTime.Now },
                    new Assunto { Nome = "Ciência", Descricao = "Estudo do mundo natural.", DataCriacao = DateTime.Now },
                };
                var movimentacoes = new List<Movimentacao>
                {
                    new Movimentacao { Nome = "Entrada", Descricao = "Relatório mensal de vendas.", DataCriacao = DateTime.Now},
                    new Movimentacao { Nome = "Saída", Descricao = "Fatura de serviços prestados", DataCriacao = DateTime.Now },
                };
                var tiposDocumento = new List<TipoDocumento>
                {
                    new TipoDocumento { Nome = "Relatório", Descricao = "Relatório mensal de vendas.", DataCriacao = DateTime.Now },
                    new TipoDocumento { Nome = "Fatura", Descricao = "Fatura de serviços prestados.", DataCriacao = DateTime.Now },
                    new TipoDocumento { Nome = "Orçamento", Descricao = "Orçamento para o próximo projeto.", DataCriacao = DateTime.Now },
                };

               // await _context.Assuntos.AddRangeAsync(assuntos);
               // await _context.Movimentacoes.AddRangeAsync(movimentacoes);
               // await _context.TiposDocumento.AddRangeAsync(tiposDocumento);
              //  await _context.SaveChangesAsync();
            }
        }
    }
}

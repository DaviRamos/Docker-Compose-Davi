using System;

namespace NetRedisASide.Models;

public class Movimentacao
{
    public int Id { get; set; }
    public string Nome { get; set; } = String.Empty;
    public string Descricao { get; set; } = String.Empty;
    public DateTime DataCriacao { get; set; }
    public DateTime DataAtualizacao { get; set; }

}

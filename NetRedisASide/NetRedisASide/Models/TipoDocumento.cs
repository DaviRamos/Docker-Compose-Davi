using System;

namespace NetRedisASide.Models;

public class TipoDocumento
{
    public int Id { get; set; }
    public string Nome { get; set; } = String.Empty;
    public string Descricao { get; set; } = String.Empty;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;    
    public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;

}

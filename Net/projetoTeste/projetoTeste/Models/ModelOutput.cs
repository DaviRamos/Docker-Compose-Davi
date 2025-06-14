using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace projetoTeste.Models
{
    public class ModelOutput
    {        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public float Score { get; set; }

       public int ModelInputId { get; set; }
       public ModelInput ModelInput { get; set; }
    }
}


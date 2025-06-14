using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace projetoTeste.Models
{
    public class ModelInput
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public float Longitude { get; set; }
        [Required]
        public float Latitude { get; set; }
        [Required]
        public float Housing_median_age { get; set; }
        [Required]
        public float Total_rooms { get; set; }
        [Required]
        public float Total_bedrooms { get; set; }
        [Required]
        public float Population { get; set; }
        [Required]
        public float Households { get; set; }
        [Required]
        public float Median_income { get; set; }
        [Required]
        public float Median_house_value { get; set; }
        [Required]
        public string? Ocean_proximity { get; set; } 
    }
}
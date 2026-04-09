using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_de_Facturacion_EPA.Models
{
    [Table("Cliente")]
    public class Cliente
    {
        [Key]
        public int IdCliente { get; set; }

        [Required]
        [StringLength(200)]
        public string NombreCompleto { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Telefono { get; set; }

        [StringLength(200)]
        public string? Correo { get; set; }

        [StringLength(300)]
        public string? Direccion { get; set; }

        public DateTime FechaRegistro { get; set; }
    }
}
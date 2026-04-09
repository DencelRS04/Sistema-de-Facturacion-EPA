using Sistema_de_Facturacion_EPA.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_de_Facturacion_EPA.Models
{
    [Table("Producto")]
    public class Producto
    {
        [Key]
        public int IdProducto { get; set; }

        [Required]
        [StringLength(200)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Descripcion { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioVenta { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal Impuesto { get; set; }

        public int IdCategoria { get; set; }

        [ForeignKey(nameof(IdCategoria))]
        public CategoriaProducto? CategoriaProducto { get; set; }
    }
}
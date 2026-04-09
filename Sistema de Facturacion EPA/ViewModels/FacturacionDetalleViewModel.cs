using System.ComponentModel.DataAnnotations;

namespace Sistema_de_Facturacion_EPA.ViewModels
{
    public class FacturaDetalleViewModel
    {
        [Required]
        public int IdProducto { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Cantidad { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal PrecioUnitario { get; set; }
    }
}
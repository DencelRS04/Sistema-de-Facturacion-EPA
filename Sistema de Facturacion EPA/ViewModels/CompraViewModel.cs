using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Sistema_de_Facturacion_EPA.ViewModels
{
    public class CompraViewModel    
    {
        [Required]
        public int IdProveedor { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaCompra { get; set; } = DateTime.Today;

        public List<CompraDetalleViewModel> Detalles { get; set; } = new();

        public List<SelectListItem> Proveedores { get; set; } = new();
        public List<SelectListItem> Productos { get; set; } = new();
    }
}
using Microsoft.AspNetCore.Mvc.Rendering;
using Sistema_de_Facturacion_EPA.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace Sistema_de_Facturacion_EPA.ViewModels
{
    public class FacturaViewModel
    {
        [Required]
        public int IdCliente { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaFactura { get; set; } = DateTime.Today;

        public List<FacturaDetalleViewModel> Detalles { get; set; } = new();

        public List<SelectListItem> Clientes { get; set; } = new();
        public List<SelectListItem> Productos { get; set; } = new();
    }
}
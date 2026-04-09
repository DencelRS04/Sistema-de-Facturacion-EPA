namespace Sistema_de_Facturacion_EPA.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalCategorias { get; set; }
        public int TotalProveedores { get; set; }
        public int TotalClientes { get; set; }
        public int TotalProductos { get; set; }
        public int TotalFacturas { get; set; }
        public int TotalCompras { get; set; }
        public int ProductosStockBajo { get; set; }
        public decimal TotalVentas { get; set; }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_de_Facturacion_EPA.Data;
using Sistema_de_Facturacion_EPA.Models;
using Sistema_de_Facturacion_EPA.ViewModels;
using System.Diagnostics;

namespace Sistema_de_Facturacion_EPA.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new DashboardViewModel
            {
                TotalCategorias = await _context.CategoriaProductos.CountAsync(),
                TotalProveedores = await _context.Proveedores.CountAsync(),
                TotalClientes = await _context.Clientes.CountAsync(),
                TotalProductos = await _context.Productos.CountAsync(),
                TotalFacturas = await _context.Facturas.CountAsync(),
                TotalCompras = await _context.Compras.CountAsync(),
                ProductosStockBajo = await _context.Inventarios.CountAsync(i => i.Stock < i.StockMinimo),
                TotalVentas = await _context.Facturas
                    .Where(f => f.Estado != "Anulada")
                    .SumAsync(f => (decimal?)f.Total) ?? 0
            };

            return View(vm);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
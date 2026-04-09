using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_de_Facturacion_EPA.Data;

namespace Sistema_de_Facturacion_EPA.Controllers
{
    public class ReportesController : Controller
    {
        private readonly AppDbContext _context;

        public ReportesController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Facturas()
        {
            var lista = await _context.Facturas
                .Include(f => f.Cliente)
                .ToListAsync();

            return View(lista);
        }

        public async Task<IActionResult> Productos()
        {
            var lista = await _context.Productos
                .Include(p => p.CategoriaProducto)
                .ToListAsync();

            return View(lista);
        }

        public async Task<IActionResult> Inventario()
        {
            var lista = await _context.Inventarios
                .Include(i => i.Producto)
                .ThenInclude(p => p.CategoriaProducto)
                .ToListAsync();

            return View(lista);
        }

        public async Task<IActionResult> Compras()
        {
            var lista = await _context.Compras
                .Include(c => c.Proveedor)
                .ToListAsync();

            return View(lista);
        }
    }
}
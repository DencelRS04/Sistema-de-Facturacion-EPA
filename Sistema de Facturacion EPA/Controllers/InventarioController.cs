using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_de_Facturacion_EPA.Data;

namespace Sistema_de_Facturacion_EPA.Controllers
{
    public class InventarioController : Controller
    {
        private readonly AppDbContext _context;

        public InventarioController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var lista = await _context.Inventarios
                .Include(i => i.Producto)
                .ThenInclude(p => p.CategoriaProducto)
                .ToListAsync();

            return View(lista);
        }
    }
}
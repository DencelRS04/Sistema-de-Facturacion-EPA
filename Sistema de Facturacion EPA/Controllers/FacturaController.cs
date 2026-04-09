using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sistema_de_Facturacion_EPA.Data;
using Sistema_de_Facturacion_EPA.Models;
using Sistema_de_Facturacion_EPA.ViewModels;

namespace Sistema_de_Facturacio_EPA.Controllers
{
    public class FacturaController : Controller
    {
        private readonly AppDbContext _context;

        public FacturaController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var lista = await _context.Facturas
                .Include(f => f.Cliente)
                .OrderByDescending(f => f.FechaFactura)
                .ToListAsync();

            return View(lista);
        }

        public async Task<IActionResult> Create()
        {
            var vm = new FacturaViewModel
            {
                FechaFactura = DateTime.Today
            };

            vm.Detalles.Add(new FacturaDetalleViewModel());
            await CargarListas(vm);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FacturaViewModel vm)
        {
            if (vm.Detalles == null || !vm.Detalles.Any())
                ModelState.AddModelError("", "Debe agregar al menos un producto.");

            if (!ModelState.IsValid)
            {
                if (vm.Detalles == null || vm.Detalles.Count == 0)
                    vm.Detalles = new List<FacturaDetalleViewModel> { new FacturaDetalleViewModel() };

                await CargarListas(vm);
                return View(vm);
            }

            decimal subtotal = vm.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario);
            decimal impuesto = 0;

            foreach (var item in vm.Detalles)
            {
                var producto = await _context.Productos.FindAsync(item.IdProducto);
                if (producto != null)
                {
                    impuesto += (item.Cantidad * item.PrecioUnitario) * (producto.Impuesto / 100);
                }
            }

            var factura = new Factura
            {
                IdCliente = vm.IdCliente,
                FechaFactura = vm.FechaFactura,
                Subtotal = subtotal,
                Impuesto = impuesto,
                Total = subtotal + impuesto,
                Estado = "Pendiente"
            };

            _context.Facturas.Add(factura);
            await _context.SaveChangesAsync();

            foreach (var item in vm.Detalles)
            {
                var detalle = new FacturaDetalle
                {
                    IdFactura = factura.IdFactura,
                    IdProducto = item.IdProducto,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = item.PrecioUnitario
                };

                _context.FacturaDetalles.Add(detalle);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var factura = await _context.Facturas
                .Include(f => f.Cliente)
                .Include(f => f.Detalles)
                .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(f => f.IdFactura == id);

            if (factura == null)
                return NotFound();

            return View(factura);
        }

        private async Task CargarListas(FacturaViewModel vm)
        {
            vm.Clientes = await _context.Clientes
                .Select(c => new SelectListItem
                {
                    Value = c.IdCliente.ToString(),
                    Text = c.NombreCompleto
                }).ToListAsync();

            vm.Productos = await _context.Productos
                .Select(p => new SelectListItem
                {
                    Value = p.IdProducto.ToString(),
                    Text = p.Nombre
                }).ToListAsync();
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using Sistema_de_Facturacion_EPA.Data;
using Sistema_de_Facturacion_EPA.ViewModels;
using System.Text.Json;

namespace Sistema_de_Facturacion_EPA.Controllers
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

            try
            {
                var detallesJson = JsonSerializer.Serialize(
                    vm.Detalles.Select(d => new
                    {
                        IdProducto = d.IdProducto,
                        Cantidad = d.Cantidad
                    })
                );

                var pIdCliente = new SqlParameter("@IdCliente", vm.IdCliente);
                var pFechaFactura = new SqlParameter("@FechaFactura", vm.FechaFactura);
                var pDetalles = new SqlParameter("@Detalles", detallesJson);

                var pIdFactura = new SqlParameter("@IdFactura", System.Data.SqlDbType.Int)
                {
                    Direction = System.Data.ParameterDirection.Output
                };

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC dbo.sp_RegistrarFactura @IdCliente, @FechaFactura, @Detalles, @IdFactura OUTPUT",
                    pIdCliente, pFechaFactura, pDetalles, pIdFactura
                );

                var idFacturaGenerada = (int)pIdFactura.Value;

                return RedirectToAction(nameof(Details), new { id = idFacturaGenerada });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ocurrió un error al guardar la factura: " + ex.Message);

                if (vm.Detalles == null || vm.Detalles.Count == 0)
                    vm.Detalles = new List<FacturaDetalleViewModel> { new FacturaDetalleViewModel() };

                await CargarListas(vm);
                return View(vm);
            }
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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sistema_de_Facturacion_EPA.Data;
using Sistema_de_Facturacion_EPA.Models;
using Sistema_de_Facturacion_EPA.ViewModels;

namespace Sistema_de_Facturacio_EPA.Controllers
{
    public class CompraController : Controller
    {
        private readonly AppDbContext _context;

        public CompraController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var lista = await _context.Compras
                .Include(c => c.Proveedor)
                .OrderByDescending(c => c.FechaCompra)
                .ToListAsync();

            return View(lista);
        }

        public async Task<IActionResult> Create()
        {
            var vm = new CompraViewModel
            {
                FechaCompra = DateTime.Today
            };

            vm.Detalles.Add(new CompraDetalleViewModel());
            await CargarListas(vm);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CompraViewModel vm)
        {
            if (vm.Detalles == null || !vm.Detalles.Any())
                ModelState.AddModelError("", "Debe agregar al menos un producto.");

            if (!ModelState.IsValid)
            {
                if (vm.Detalles == null || vm.Detalles.Count == 0)
                    vm.Detalles = new List<CompraDetalleViewModel> { new CompraDetalleViewModel() };

                await CargarListas(vm);
                return View(vm);
            }

            var compra = new Compra
            {
                IdProveedor = vm.IdProveedor,
                FechaCompra = vm.FechaCompra,
                Total = vm.Detalles.Sum(d => d.Cantidad * d.CostoUnitario)
            };

            _context.Compras.Add(compra);
            await _context.SaveChangesAsync();

            foreach (var item in vm.Detalles)
            {
                var detalle = new CompraDetalle
                {
                    IdCompra = compra.IdCompra,
                    IdProducto = item.IdProducto,
                    Cantidad = item.Cantidad,
                    CostoUnitario = item.CostoUnitario
                };

                _context.CompraDetalles.Add(detalle);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task CargarListas(CompraViewModel vm)
        {
            vm.Proveedores = await _context.Proveedores
                .Select(p => new SelectListItem
                {
                    Value = p.IdProveedor.ToString(),
                    Text = p.Nombre
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
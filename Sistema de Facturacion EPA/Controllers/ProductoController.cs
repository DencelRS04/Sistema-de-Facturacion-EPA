using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sistema_de_Facturacion_EPA.Data;
using Sistema_de_Facturacion_EPA.Models;

namespace Sistema_de_Facturacion_EPA.Controllers
{
    public class ProductoController : Controller
    {
        private readonly AppDbContext _context;

        public ProductoController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var lista = await _context.Productos
                .Include(p => p.CategoriaProducto)
                .ToListAsync();

            return View(lista);
        }

        public async Task<IActionResult> Create()
        {
            await CargarCategorias();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Producto model)
        {
            if (!ModelState.IsValid)
            {
                await CargarCategorias(model.IdCategoria);
                return View(model);
            }

            _context.Productos.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound();

            await CargarCategorias(producto.IdCategoria);
            return View(producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Producto model)
        {
            if (!ModelState.IsValid)
            {
                await CargarCategorias(model.IdCategoria);
                return View(model);
            }

            _context.Productos.Update(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var producto = await _context.Productos
                .Include(p => p.CategoriaProducto)
                .FirstOrDefaultAsync(p => p.IdProducto == id);

            if (producto == null) return NotFound();

            return View(producto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var producto = await _context.Productos.FindAsync(id);

                if (producto == null)
                {
                    TempData["Error"] = "El producto no existe.";
                    return RedirectToAction(nameof(Index));
                }

                bool tieneInventario = await _context.Inventarios.AnyAsync(i => i.IdProducto == id);
                bool tieneCompras = await _context.CompraDetalles.AnyAsync(cd => cd.IdProducto == id);
                bool tieneFacturas = await _context.FacturaDetalles.AnyAsync(fd => fd.IdProducto == id);

                if (tieneInventario || tieneCompras || tieneFacturas)
                {
                    TempData["Error"] = "No se puede eliminar el producto porque tiene registros relacionados.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Producto eliminado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "No se puede eliminar el producto porque tiene registros relacionados.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ocurrió un error al eliminar el producto: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        private async Task CargarCategorias(int? seleccionada = null)
        {
            var categorias = await _context.CategoriaProductos.ToListAsync();
            ViewBag.IdCategoria = new SelectList(categorias, "IdCategoria", "Nombre", seleccionada);
        }
    }
}
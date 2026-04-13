using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_de_Facturacion_EPA.Data;
using Sistema_de_Facturacion_EPA.Models;

namespace Sistema_de_Facturacion_EPA.Controllers
{
    public class ProveedorController : Controller
    {
        private readonly AppDbContext _context;

        public ProveedorController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var lista = await _context.Proveedores.ToListAsync();
            return View(lista);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Proveedor model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.FechaRegistro = DateTime.Now;
            _context.Proveedores.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor == null)
                return NotFound();

            return View(proveedor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Proveedor model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _context.Proveedores.Update(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor == null)
                return NotFound();

            return View(proveedor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var proveedor = await _context.Proveedores.FindAsync(id);

                if (proveedor == null)
                {
                    TempData["Error"] = "El proveedor no existe.";
                    return RedirectToAction(nameof(Index));
                }

                bool tieneCompras = await _context.Compras.AnyAsync(c => c.IdProveedor == id);

                if (tieneCompras)
                {
                    TempData["Error"] = "No se puede eliminar el proveedor porque tiene compras registradas.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Proveedores.Remove(proveedor);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Proveedor eliminado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ocurrió un error al eliminar el proveedor: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
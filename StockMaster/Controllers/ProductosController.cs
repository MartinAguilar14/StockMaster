using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StockMaster.Data;
using StockMaster.Models;

namespace StockMaster.Controllers
{
    [Authorize]
    public class ProductosController : Controller
    {
        private readonly CLS_DbContext _context;

        public ProductosController(CLS_DbContext context)
        {
            _context = context;
        }

        // GET: Productos
        public async Task<IActionResult> Index(string search, int? categoriaId, string orden)
        {
            var productos = _context.Productos.Include(p => p.Categoria).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                productos = productos.Where(p => p.Nombre.Contains(search) || p.SKU.Contains(search));
                
            }

            if (categoriaId != null)
            {
                productos = productos.Where(p => p.CategoriaId == categoriaId);
            }

            switch (orden)
            {
                case "precio_asc":
                    productos = productos.OrderBy(p => p.Precio);
                    break;
                case "precio_desc":
                    productos = productos.OrderByDescending(p => p.Precio);
                    break;
                case "stock_asc":
                    productos = productos.OrderBy(p => p.Stock);
                    break;
                case "stock_desc":
                    productos = productos.OrderByDescending(p => p.Stock);
                    break;
            }

            ViewBag.Categorias = new SelectList(_context.Categorias, "Id", "Nombre");

            return View(await productos.ToListAsync());

            //var cLS_DbContext = _context.Productos.Include(p => p.Categoria);
            //return View(await cLS_DbContext.ToListAsync());
        }

        // GET: Productos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // GET: Productos/Create
        public IActionResult Create()
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nombre");
            return View();
        }

        // POST: Productos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,SKU,Precio,Stock,CategoriaId")] Producto producto)
        {
            bool existeSKU = _context.Productos.Any(p => p.SKU == producto.SKU);

            if (existeSKU)
            {
                ModelState.AddModelError("SKU", "El SKU ya existe");
            }

            if (ModelState.IsValid)
            {
                _context.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nombre", producto.CategoriaId);
            return View(producto);
        }

        // GET: Productos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nombre", producto.CategoriaId);
            return View(producto);
        }

        // POST: Productos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,SKU,Precio,Stock,CategoriaId")] Producto producto)
        {
            bool existeSKU = _context.Productos.Any(p => p.SKU == producto.SKU && p.Id != producto.Id);

            if (existeSKU)
            {
                ModelState.AddModelError("SKU", "El SKU ya existe");
            }

            if (id != producto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nombre", producto.CategoriaId);
            return View(producto);
        }

        // GET: Productos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                _context.Productos.Remove(producto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.Id == id);
        }
    }
}

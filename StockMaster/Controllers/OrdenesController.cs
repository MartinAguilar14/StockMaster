using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using StockMaster.Data;
using StockMaster.Models;

namespace StockMaster.Controllers
{
    [Authorize]
    public class OrdenesController : Controller
    {
        private readonly CLS_DbContext _context;

        public OrdenesController(CLS_DbContext context)
        {
            _context = context;
        }

        // GET: Ordenes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Ordenes.ToListAsync());
        }

        // GET: Ordenes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordenes = await _context.Ordenes
                .Include(o => o.Detalles)
                .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ordenes == null)
            {
                return NotFound();
            }

            return View(ordenes);
        }

        // GET: Ordenes/Create
        public IActionResult Create()
        {
            ViewBag.Productos = new SelectList(_context.Productos, "Id", "Nombre");
            ViewBag.Productos = _context.Productos
            .Select(p => new {
                p.Id,
                p.Nombre,
                p.Stock
            }).ToList();
            return View();
        }

        // POST: Ordenes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(List<int> productoId, List<int> cantidad)
        {
            var orden = new Ordenes
            {
                Fecha = DateTime.Now,
                Folio = Guid.NewGuid().ToString(),
                Total = 0
            };

            _context.Ordenes.Add(orden);
            await _context.SaveChangesAsync();

            for (int i = 0; i < productoId.Count; i++)
            {
                if (cantidad[i] <= 0)
                {
                    return BadRequest("Cantidad inválida");
                }

                var producto = await _context.Productos.FindAsync(productoId[i]);

                if (producto.Stock < cantidad[i])
                {
                    return BadRequest("Stock insuficiente");
                }

                producto.Stock -= cantidad[i];

                var detalle = new OrdenDetalle
                {
                    OrdenId = orden.Id,
                    ProductoId = productoId[i],
                    Cantidad = cantidad[i],
                    PrecioUnitario = producto.Precio
                };

                orden.Total += producto.Precio * cantidad[i];

                _context.OrdenDetalles.Add(detalle);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
            //if (ModelState.IsValid)
            //{
            //    _context.Add(ordenes);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}
            //return View(ordenes);
        }

        // GET: Ordenes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordenes = await _context.Ordenes.FindAsync(id);
            if (ordenes == null)
            {
                return NotFound();
            }
            return View(ordenes);
        }

        // POST: Ordenes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Folio,Fecha,Total")] Ordenes ordenes)
        {
            if (id != ordenes.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ordenes);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrdenesExists(ordenes.Id))
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
            return View(ordenes);
        }

        // GET: Ordenes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordenes = await _context.Ordenes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ordenes == null)
            {
                return NotFound();
            }

            return View(ordenes);
        }

        // POST: Ordenes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ordenes = await _context.Ordenes.FindAsync(id);
            if (ordenes != null)
            {
                _context.Ordenes.Remove(ordenes);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrdenesExists(int id)
        {
            return _context.Ordenes.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Pdf(int id)
        {
            var orden = await _context.Ordenes
                .Include(o => o.Detalles)
                .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(o => o.Id == id);

            return new ViewAsPdf("GenerarPDF", orden)
            {
                FileName = $"Orden_{orden.Folio}.pdf"
            };
        }
    }
}

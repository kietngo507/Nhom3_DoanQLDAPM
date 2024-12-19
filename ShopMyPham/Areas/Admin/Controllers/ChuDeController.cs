using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SlugGenerator;
using ITShop.Models;
using Microsoft.AspNetCore.Authorization;
namespace ITShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ChuDeController : Controller
    {
        private readonly ITShopDbContext _context;

        public ChuDeController(ITShopDbContext context)
        {
            _context = context;
        }

        // GET: ChuDe
        public async Task<IActionResult> Index()
        {
            return View(await _context.ChuDe.ToListAsync());
        }

        // GET: ChuDe/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ChuDe/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,TenChuDe,TenChuDeKhongDau")] ChuDe chuDe)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(chuDe.TenChuDeKhongDau))
                {
                    chuDe.TenChuDeKhongDau = chuDe.TenChuDe.GenerateSlug();
                }

                _context.Add(chuDe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(chuDe);
        }

        // GET: ChuDe/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chuDe = await _context.ChuDe.FindAsync(id);
            if (chuDe == null)
            {
                return NotFound();
            }
            return View(chuDe);
        }

        // POST: ChuDe/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,TenChuDe,TenChuDeKhongDau")] ChuDe chuDe)
        {
            if (id != chuDe.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(chuDe.TenChuDeKhongDau))
                    {
                        chuDe.TenChuDeKhongDau = chuDe.TenChuDe.GenerateSlug();
                    }
                    _context.Update(chuDe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChuDeExists(chuDe.ID))
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
            return View(chuDe);
        }

        // GET: ChuDe/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chuDe = await _context.ChuDe
            .FirstOrDefaultAsync(m => m.ID == id);
            if (chuDe == null)
            {
                return NotFound();
            }

            return View(chuDe);
        }

        // POST: ChuDe/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chuDe = await _context.ChuDe.FindAsync(id);
            if (chuDe != null)
            {
                _context.ChuDe.Remove(chuDe);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChuDeExists(int id)
        {
            return _context.ChuDe.Any(e => e.ID == id);
        }
    }
}
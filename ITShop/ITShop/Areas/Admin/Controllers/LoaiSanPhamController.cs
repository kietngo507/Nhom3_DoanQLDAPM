using System;
using System.Collections.Generic;
using System.Linq;
using SlugGenerator;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ITShop.Models;
using Microsoft.AspNetCore.Authorization;

namespace ITShop.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class LoaiSanPhamController : Controller
    {
        private readonly ITShopDbContext _context;

        public LoaiSanPhamController(ITShopDbContext context)
        {
            _context = context;
        }

        // GET: LoaiSanPham
        public async Task<IActionResult> Index()
        {
            return View(await _context.LoaiSanPham.ToListAsync());
        }

        // GET: LoaiSanPham/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LoaiSanPham/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,TenLoai,TenLoaiKhongDau")] LoaiSanPham loaiSanPham)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(loaiSanPham.TenLoaiKhongDau))
                {
                    loaiSanPham.TenLoaiKhongDau = loaiSanPham.TenLoai.GenerateSlug();
                }

                _context.Add(loaiSanPham);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(loaiSanPham);
        }

        // GET: LoaiSanPham/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loaiSanPham = await _context.LoaiSanPham.FindAsync(id);
            if (loaiSanPham == null)
            {
                return NotFound();
            }
            return View(loaiSanPham);
        }

        // POST: LoaiSanPham/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,TenLoai,TenLoaiKhongDau")] LoaiSanPham loaiSanPham)
        {
            if (id != loaiSanPham.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(loaiSanPham.TenLoaiKhongDau))
                    {
                        loaiSanPham.TenLoaiKhongDau = loaiSanPham.TenLoai.GenerateSlug();
                    }
                    _context.Update(loaiSanPham);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoaiSanPhamExists(loaiSanPham.ID))
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
            return View(loaiSanPham);
        }

        // GET: LoaiSanPham/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loaiSanPham = await _context.LoaiSanPham
            .FirstOrDefaultAsync(m => m.ID == id);
            if (loaiSanPham == null)
            {
                return NotFound();
            }
            return View(loaiSanPham);
        }

        // POST: LoaiSanPham/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loaiSanPham = await _context.LoaiSanPham.FindAsync(id);
            if (loaiSanPham != null)
            {
                _context.LoaiSanPham.Remove(loaiSanPham);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LoaiSanPhamExists(int id)
        {
            return _context.LoaiSanPham.Any(e => e.ID == id);
        }
    }

}

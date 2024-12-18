using ITShop.Logic;
using ITShop.Models;
using Microsoft.AspNetCore.Mvc;
namespace ITShop.Controllers
{
    public class GioHangController : Controller
    {
        private readonly ITShopDbContext _context;

        public GioHangController(ITShopDbContext context)
        {
            _context = context;
        }

        // GET: Index
        public IActionResult Index()
        {
            GioHangLogic gioHangLogic = new GioHangLogic(_context);
            var gioHang = gioHangLogic.LayGioHang();
            if (gioHang.Count == 0)
                return View("GioHangRong");
            else
            {
                decimal tongTien = gioHangLogic.LayTongTienSanPham();
                TempData["TongTien"] = tongTien;
                return View(gioHang);
            }
        }

        // GET: Them
        public IActionResult Them(int id)
        {
            GioHangLogic gioHangLogic = new GioHangLogic(_context);
            var sanPham = _context.SanPham.Where(r => r.ID == id).SingleOrDefault();
            if (sanPham != null)
            {
                gioHangLogic.ThemSanPham(sanPham.ID);
            }
            return RedirectToAction("Index", "Home", new { Area = "" });
        }

        // GET: Giam
        public IActionResult Giam(string? id)
        {
            if (id == null || _context.GioHang == null)
            {
                return NotFound();
            }
            GioHangLogic gioHangLogic = new GioHangLogic(_context);
            gioHangLogic.CapNhatSoLuong(id, true);
            return RedirectToAction("Index", "GioHang", new { Area = "" });
        }

        // GET: Tang
        public IActionResult Tang(string? id)
        {
            if (id == null || _context.GioHang == null)
            {
                return NotFound();
            }
            GioHangLogic gioHangLogic = new GioHangLogic(_context);
            gioHangLogic.CapNhatSoLuong(id, false);
            return RedirectToAction("Index", "GioHang", new { Area = "" });
        }

        // GET: Xoa
        public IActionResult Xoa(string? id)
        {
            if (id == null || _context.GioHang == null)
            {
                return NotFound();
            }
            GioHangLogic gioHangLogic = new GioHangLogic(_context);
            gioHangLogic.XoaSanPham(id);
            return RedirectToAction("Index", "GioHang", new { Area = "" });
        }

        // POST: CapNhat
        [HttpPost]
        public IActionResult CapNhat(IDictionary<string, string> SoLuongTrongGio)
        {
            foreach (KeyValuePair<string, string> entry in SoLuongTrongGio)
            {
                GioHangLogic gioHangLogic = new GioHangLogic(_context);
                gioHangLogic.CapNhatSoLuong(entry.Key, Convert.ToInt32(entry.Value));
            }
            return RedirectToAction("Index", "GioHang", new { Area = "" });
        }
    }
}

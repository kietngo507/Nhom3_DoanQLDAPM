using ITShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace ITShop.Controllers
{
    public class BaiVietController : Controller
    {
        private readonly ITShopDbContext _context;
        private readonly HttpContext _httpContext = new HttpContextAccessor().HttpContext;

        public BaiVietController(ITShopDbContext context)
        {
            _context = context;
        }

        // GET: Index
        [Route("tat-ca-bai-viet/{trang?}")]
        public IActionResult Index(int? trang)
        {
            var danhSach = LayDanhSachBaiViet(trang ?? 1);
            if (danhSach.BaiViet.Count == 0)
                return NotFound();
            else
                return View(danhSach);
        }

        private PhanTrangBaiViet LayDanhSachBaiViet(int trangHienTai)
        {
            int maxRows = 12;

            PhanTrangBaiViet phanTrang = new PhanTrangBaiViet();
            phanTrang.BaiViet = _context.BaiViet
            .Include(s => s.NguoiDung)
            .Include(s => s.ChuDe)
            .Where(r => r.KiemDuyet == true && r.HienThi == true)
            .OrderByDescending(r => r.NgayDang)
            .Skip((trangHienTai - 1) * maxRows)
            .Take(maxRows).ToList();

            decimal tongSoTrang = Convert.ToDecimal(_context.BaiViet.Count()) / Convert.ToDecimal(maxRows);
            phanTrang.TongSoTrang = (int)Math.Ceiling(tongSoTrang);
            phanTrang.TrangHienTai = trangHienTai;

            return phanTrang;
        }

        // GET: ChuDe
        [Route("bai-viet/{tenChuDe}/{trang?}")]
        public IActionResult ChuDe(string tenChuDe, int? trang)
        {
            var danhSachPhanLoai = LayDanhSachBaiVietTheoChuDe(tenChuDe, trang ?? 1);
            if (danhSachPhanLoai.BaiViet.Count == 0)
                return NotFound();
            else
                return View(danhSachPhanLoai);
        }

        private PhanTrangBaiViet LayDanhSachBaiVietTheoChuDe(string tenChuDe, int trangHienTai)
        {
            int maxRows = 12;

            var baiVietTheoChuDe = _context.BaiViet
            .Include(s => s.NguoiDung)
            .Include(s => s.ChuDe)
            .Where(r => r.KiemDuyet == true && r.HienThi == true && r.ChuDe.TenChuDeKhongDau == tenChuDe);

            PhanTrangBaiViet phanTrang = new PhanTrangBaiViet();
            phanTrang.BaiViet = baiVietTheoChuDe.OrderByDescending(r => r.NgayDang)
            .Skip((trangHienTai - 1) * maxRows)
            .Take(maxRows).ToList();

            decimal tongSoTrang = Convert.ToDecimal(baiVietTheoChuDe.Count()) / Convert.ToDecimal(maxRows);
            phanTrang.TongSoTrang = (int)Math.Ceiling(tongSoTrang);
            phanTrang.TrangHienTai = trangHienTai;

            return phanTrang;
        }

        // GET: ChiTiet
        [Route("bai-viet/{tenChuDe}/{tieuDe}.html")]
        public IActionResult ChiTiet(string tenChuDe, string tieuDe)
        {
            var baiViet = _context.BaiViet
            .Include(s => s.NguoiDung)
            .Include(s => s.ChuDe)
            .Include(s => s.BinhLuanBaiViet)
            .Where(r => r.KiemDuyet == true && r.HienThi == true && r.TieuDeKhongDau == tieuDe).SingleOrDefault();
            if (baiViet == null)
                return NotFound();
            else
            {
                // Xử lý tăng lượt xem
                string _sessionKey = "DaXemBaiViet_" + baiViet.ID;

                if (string.IsNullOrEmpty(_httpContext.Session.GetString(_sessionKey)))
                {
                    baiViet.LuotXem = baiViet.LuotXem + 1;
                    _context.Update(baiViet);
                    _context.SaveChanges();
                    _httpContext.Session.SetString(_sessionKey, "1");
                }

                // Lấy bài viết cùng chuyên mục
                var baiVietCungChuyenMuc = _context.BaiViet
                .Include(s => s.NguoiDung)
               .Include(s => s.ChuDe)
               .Where(r => r.KiemDuyet == true && r.HienThi == true && r.ChuDeID == baiViet.ChuDeID && r.ID != baiViet.ID)
                .OrderByDescending(r => r.NgayDang)
               .Take(4);
                ViewData["BaiVietCungChuyenMuc"] = baiVietCungChuyenMuc;

                return View(baiViet);
            }
        }
    }
}

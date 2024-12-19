using ITShop.Logic;
using ITShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using BC = BCrypt.Net.BCrypt;
namespace ITShop.Controllers
{
    [Authorize(Roles = "Admin, User")]
    public class KhachHangController : Controller
    {
        private readonly ITShopDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMailLogic _mailLogic;

        public KhachHangController(ITShopDbContext context, IHttpContextAccessor httpContextAccessor, IMailLogic mailLogic)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _mailLogic = mailLogic;
        }

        // GET: DonHangCuaToi
        public async Task<IActionResult> DonHangCuaToi()
        {
            int maNguoiDung = Convert.ToInt32(_httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value);
            var datHang = _context.DatHang.Where(r => r.NguoiDungID == maNguoiDung)
            .Include(d => d.NguoiDung)
            .Include(d => d.TinhTrang)
            .Include(d => d.DatHang_ChiTiet)
            .ThenInclude(s => s.SanPham);
            return View(await datHang.ToListAsync());
        }
        // GET: Index
        public IActionResult Index(string? successMessage)
        {
            int maNguoiDung = Convert.ToInt32(_httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value);
            var nguoiDung = _context.NguoiDung.Where(r => r.ID == maNguoiDung).Include(s => s.DatHang).SingleOrDefault();
            if (nguoiDung == null)
            {
                return NotFound();
            }
            int soLuongDonHang = nguoiDung.DatHang.Count();
            TempData["SoLuongDonHang"] = soLuongDonHang;
            if (!string.IsNullOrEmpty(successMessage))
                TempData["ThongBao"] = successMessage;
            return View(new NguoiDung_ChinhSua(nguoiDung));
        }
        // POST: CapNhatHoSo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatHoSo(int id, [Bind("ID,HoVaTen,Email,DienThoai,DiaChi,TenDangNhap,MatKhau,XacNhanMatKhau")] NguoiDung_ChinhSua nguoiDung)
        {
            if (id != nguoiDung.ID)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var n = _context.NguoiDung.Find(id);
                    // Giữ nguyên mật khẩu cũ
                    if (nguoiDung.MatKhau == null)
                    {
                        n.ID = nguoiDung.ID;
                        n.HoVaTen = nguoiDung.HoVaTen;
                        n.DienThoai = nguoiDung.DienThoai;
                        n.DiaChi = nguoiDung.DiaChi;
                        n.TenDangNhap = n.TenDangNhap;
                        n.XacNhanMatKhau = n.MatKhau;
                        n.Quyen = n.Quyen;
                    }
                    else // Cập nhật mật khẩu mới
                    {
                        n.ID = nguoiDung.ID;
                        n.HoVaTen = nguoiDung.HoVaTen;
                        n.DienThoai = nguoiDung.DienThoai;
                        n.DiaChi = nguoiDung.DiaChi;
                        n.TenDangNhap = n.TenDangNhap;
                        n.MatKhau = BC.HashPassword(nguoiDung.MatKhau);
                        n.XacNhanMatKhau = BC.HashPassword(nguoiDung.MatKhau);
                        n.Quyen = n.Quyen;
                    }
                    _context.Update(n);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message.ToString());
                }
                return RedirectToAction("Index", "KhachHang", new { Area = "", successMessage = "Đã cập nhật thông tin thành công." });
            }
            return View("Index", nguoiDung);
        }


        // GET: DatHang
        public IActionResult DatHang()
        {
            GioHangLogic gioHangLogic = new GioHangLogic(_context);
            var gioHang = gioHangLogic.LayGioHang();
            decimal tongTien = gioHangLogic.LayTongTienSanPham();
            TempData["TongTien"] = tongTien;
            return View(gioHang);
        }

        // POST: DatHang
        [HttpPost]
        public async Task<IActionResult> DatHang(DatHang datHang)
        {
            GioHangLogic gioHangLogic = new GioHangLogic(_context);
            var gioHang = gioHangLogic.LayGioHang();

            if (string.IsNullOrWhiteSpace(datHang.DienThoaiGiaoHang) || string.IsNullOrWhiteSpace(datHang.DiaChiGiaoHang))
            {
                decimal tongTien = gioHangLogic.LayTongTienSanPham();
                TempData["TongTien"] = tongTien;
                TempData["ThongBaoLoi"] = "Thông tin giao hàng không được bỏ trống.";
                return View(gioHang);
            }
            else
            {
                DatHang dh = new DatHang();
                dh.NguoiDungID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value);
                dh.TinhTrangID = 1; // Đơn hàng mới
                dh.DienThoaiGiaoHang = datHang.DienThoaiGiaoHang;
                dh.DiaChiGiaoHang = datHang.DiaChiGiaoHang;
                dh.NgayDatHang = DateTime.Now;
                _context.DatHang.Add(dh);
                await _context.SaveChangesAsync();

                foreach (var item in gioHang)
                {
                    DatHang_ChiTiet ct = new DatHang_ChiTiet();
                    ct.DatHangID = dh.ID;
                    ct.SanPhamID = item.SanPhamID;
                    ct.SoLuong = Convert.ToInt16(item.SoLuongTrongGio);
                    ct.DonGia = item.SanPham.DonGia;
                    _context.DatHang_ChiTiet.Add(ct);
                    await _context.SaveChangesAsync();
                }

                // Gởi email
                try
                {
                    MailInfo mailInfo = new MailInfo();
                    mailInfo.Subject = "Đặt hàng thành công tại ITShop.Com.Vn";
                    var datHangInfo = _context.DatHang.Where(r => r.ID == dh.ID)
                    .Include(s => s.NguoiDung)
                   .Include(s => s.DatHang_ChiTiet).SingleOrDefault();
                    await _mailLogic.GoiEmailDatHangThanhCong(datHangInfo, mailInfo);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message.ToString());
                }

                return RedirectToAction("DatHangThanhCong", "KhachHang", new { Area = "" });
            }
        }

        // GET: DatHangThanhCong
        public IActionResult DatHangThanhCong()
        {
            // Xóa giỏ hàng sau khi hoàn tất đặt hàng
            GioHangLogic gioHangLogic = new GioHangLogic(_context);
            gioHangLogic.XoaTatCa();

            return View();
        }
    }
}
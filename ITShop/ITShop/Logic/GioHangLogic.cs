using ITShop.Models;
using Microsoft.EntityFrameworkCore;
namespace ITShop.Logic
{
    public class GioHangLogic
    {
        private string _tenDangNhap { get; set; }
        private const string _sessionKey = "Cart";
        private readonly ITShopDbContext _context;
        private readonly HttpContext _httpContext = new HttpContextAccessor().HttpContext;

        public GioHangLogic(ITShopDbContext context)
        {
            _context = context;
        }

        // Lấy tên đăng nhập của người dùng
        // Nếu chưa đăng nhập thì phát sinh mã ngẫu nhiên
        // Nếu người dùng đã đăng nhập thì trả về tên đăng nhập.
        public string LayTenDangNhap()
        {
            if (string.IsNullOrEmpty(_httpContext.Session.GetString(_sessionKey)))
            {
                if (!string.IsNullOrWhiteSpace(_httpContext.User.Identity.Name))
                {
                    _httpContext.Session.SetString(_sessionKey, _httpContext.User.Identity.Name);
                }
                else
                {
                    Guid guid = Guid.NewGuid();
                    _httpContext.Session.SetString(_sessionKey, guid.ToString());
                }
            }
            return _httpContext.Session.GetString(_sessionKey);
        }

        public decimal LayTongTienSanPham()
        {
            _tenDangNhap = LayTenDangNhap();
            decimal? tongTien = decimal.Zero;
            tongTien = (decimal?)(from r in _context.GioHang
                                  where r.TenDangNhap == _tenDangNhap
                                  select (decimal?)r.SoLuongTrongGio * r.SanPham.DonGia).Sum();
            return tongTien ?? decimal.Zero;
        }

        public int LayTongSoLuong()
        {
            _tenDangNhap = LayTenDangNhap();
            int? tongSoLuong = 0;
            tongSoLuong = (int?)(from r in _context.GioHang
                                 where r.TenDangNhap == _tenDangNhap
                                 select (int?)r.SoLuongTrongGio).Sum();
            return tongSoLuong ?? 0;
        }

        public List<GioHang> LayGioHang()
        {
            _tenDangNhap = LayTenDangNhap();
            return _context.GioHang.Where(r => r.TenDangNhap == _tenDangNhap).Include(s => s.SanPham).ToList();
        }

        public void ThemSanPham(int maSanPham)
        {
            _tenDangNhap = LayTenDangNhap();
            var gioHang = _context.GioHang.SingleOrDefault(r => r.TenDangNhap == _tenDangNhap && r.SanPhamID == maSanPham);
            if (gioHang == null)
            {
                gioHang = new GioHang
                {
                    ID = Guid.NewGuid().ToString(),
                    TenDangNhap = _tenDangNhap,
                    SanPhamID = maSanPham,
                    SanPham = _context.SanPham.SingleOrDefault(r => r.ID == maSanPham),
                    SoLuongTrongGio = 1,
                    ThoiGian = DateTime.Now
                };
                _context.GioHang.Add(gioHang);
            }
            else
            {
                gioHang.SoLuongTrongGio++;
            }
            _context.SaveChanges();
        }

        public void XoaSanPham(string id)
        {
            try
            {
                var gioHang = _context.GioHang.Find(id);
                if (gioHang != null)
                {
                    _context.GioHang.Remove(gioHang);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }

        public void CapNhatSoLuong(string id, int soLuong)
        {
            try
            {
                var gioHang = _context.GioHang.Find(id);
                if (gioHang != null)
                {
                    if (soLuong <= 0)
                        gioHang.SoLuongTrongGio = 1;
                    else if (soLuong > 10)
                        gioHang.SoLuongTrongGio = 10;
                    else
                        gioHang.SoLuongTrongGio = soLuong;
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }

        public void CapNhatSoLuong(string id, bool giamSoLuong = false)
        {
            try
            {
                var gioHang = _context.GioHang.Find(id);
                if (gioHang != null)
                {
                    if (giamSoLuong)
                    {
                        // Nếu số lượng là 1 thì không giảm được nữa
                        if (gioHang.SoLuongTrongGio > 1)
                        {
                            gioHang.SoLuongTrongGio--;
                            _context.SaveChanges();
                        }
                    }
                    else
                    {
                        // Không được tăng vượt quá 10 sản phẩm
                        if (gioHang.SoLuongTrongGio < 10)
                        {
                            gioHang.SoLuongTrongGio++;
                            _context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }

        public void XoaTatCa()
        {
            try
            {
                _tenDangNhap = LayTenDangNhap();
                var gioHang = _context.GioHang.Where(r => r.TenDangNhap == _tenDangNhap);
                foreach (var item in gioHang)
                {
                    _context.GioHang.Remove(item);
                }
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }
    }
}
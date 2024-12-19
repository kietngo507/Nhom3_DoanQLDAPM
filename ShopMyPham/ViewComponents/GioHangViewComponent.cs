using ITShop.Logic;
using ITShop.Models;
using Microsoft.AspNetCore.Mvc;
namespace ITShop.ViewComponents
{
    public class GioHangViewComponent : ViewComponent
    {
        private readonly ITShopDbContext _context;

        public GioHangViewComponent(ITShopDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            GioHangLogic gioHangLogic = new GioHangLogic(_context);
            decimal tongTien = gioHangLogic.LayTongTienSanPham();
            decimal tongSoLuong = gioHangLogic.LayTongSoLuong();
            TempData["TopMenu_TongTien"] = tongTien;
            TempData["TopMenu_TongSoLuong"] = tongSoLuong;
            return View("Default");
        }
    }
}

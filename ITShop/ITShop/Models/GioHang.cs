using System.ComponentModel.DataAnnotations;
namespace ITShop.Models
{
    public class GioHang
    {
        [Key]
        [StringLength(255)]
        public string ID { get; set; }

        [StringLength(255)]
        public string TenDangNhap { get; set; }

        public int SanPhamID { get; set; }
        public int SoLuongTrongGio { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = false)]
        public DateTime ThoiGian { get; set; }

        public SanPham? SanPham { get; set; }
    }
}
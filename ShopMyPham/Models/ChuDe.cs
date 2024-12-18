using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
namespace ITShop.Models
{
    public class ChuDe
    {
        [DisplayName("Mã chủ đề")]
        public int ID { get; set; }

        [StringLength(255)]
        [Required(ErrorMessage = "Tên chủ đề không được bỏ trống.")]
        [DisplayName("Tên chủ đề")]
        public string TenChuDe { get; set; }

        [StringLength(255)]
        [DisplayName("Tên chủ đề không dấu")]
        public string? TenChuDeKhongDau { get; set; }

        public ICollection<BaiViet>? BaiViet { get; set; }
    }
}
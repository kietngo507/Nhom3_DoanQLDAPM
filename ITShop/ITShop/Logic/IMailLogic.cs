using ITShop.Models;
namespace ITShop.Logic
{
    public interface IMailLogic
    {
        Task GoiEmail(MailInfo mailInfo);
        Task GoiEmailDatHangThanhCong(DatHang datHang, MailInfo mailInfo);
    }
}
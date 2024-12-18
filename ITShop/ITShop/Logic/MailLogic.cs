using ITShop.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
namespace ITShop.Logic
{
    public class MailLogic : IMailLogic
    {
        private readonly MailSettings _mailSettings;

        public MailLogic(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task GoiEmail(MailInfo mailInfo)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Address));
            email.To.Add(new MailboxAddress(null, mailInfo.ToEmail));
            email.Subject = mailInfo.Subject;
            var builder = new BodyBuilder();
            if (mailInfo.Attachments != null)
            {
                byte[] fileBytes;
                foreach (var file in mailInfo.Attachments)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            fileBytes = ms.ToArray();
                        }
                        builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                    }
                }
            }
            builder.HtmlBody = mailInfo.Body;
            email.Body = builder.ToMessageBody();

            var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Address, _mailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }

        public async Task GoiEmailDatHangThanhCong(DatHang datHang, MailInfo mailInfo)
        {
            string FilePath = Directory.GetCurrentDirectory() + "\\Emails\\DatHangThanhCong.html";
            StreamReader str = new StreamReader(FilePath);
            string MailText = str.ReadToEnd();
            str.Close();

            string chiTiet = "";
            int stt = 1;
            decimal tongTien = 0;
            foreach (var item in datHang.DatHang_ChiTiet)
            {
                chiTiet += "<tr>" +
                "<td>" + stt + "</td>" +
                "<td>" + item.SanPham.TenSanPham + "</td>" +
                "<td>" + item.SoLuong + "</td>" +
                "<td style='text-align:right'>" + string.Format("{0:N0}", item.DonGia) + "</td>" +
                "<td style='text-align:right'>" + string.Format("{0:N0}", (item.SoLuong * item.DonGia)) + "<sup>đ</sup></td>" +
                "</tr>";
                tongTien += item.SoLuong * item.DonGia;
                stt++;
            }

            MailText = MailText.Replace("[HoVaTen]", datHang.NguoiDung.HoVaTen)
            .Replace("[DienThoaiGiaoHang]", datHang.DienThoaiGiaoHang)
            .Replace("[DiaChiGiaoHang]", datHang.DiaChiGiaoHang)
            .Replace("[DatHang_ChiTiet]", chiTiet)
            .Replace("[TongTienSanPham]", string.Format("{0:N0}", tongTien));

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Address));
            email.To.Add(new MailboxAddress(datHang.NguoiDung.HoVaTen, datHang.NguoiDung.Email));
            email.Subject = mailInfo.Subject;
            var builder = new BodyBuilder();
            builder.HtmlBody = MailText;
            email.Body = builder.ToMessageBody();

            var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Address, _mailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
    }
}

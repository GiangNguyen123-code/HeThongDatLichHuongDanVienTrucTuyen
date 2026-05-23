using HeThongDatLich.Data;
using HeThongDatLich.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeThongDatLich.Controllers.KhachHangControllers
{
    public class DanhGiaController : Controller
    {
        private readonly AppDbContext _context;

        public DanhGiaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuiDanhGia(int maDatLich, int soSao, string noiDung)
        {
            var donDatLich = await _context.DonDatLichs.FindAsync(maDatLich);

            // Trang thái 3 = Hoàn thành (Ví dụ)
            if (donDatLich == null || donDatLich.TrangThai != 3)
            {
                TempData["Error"] = "Chuyến đi chưa hoàn thành, bạn chưa thể đánh giá.";
                return RedirectToAction("Index", "Home");
            }

            bool daDanhGia = await _context.DanhGias.AnyAsync(d => d.MaDatLich == maDatLich);
            if (daDanhGia)
            {
                TempData["Error"] = "Bạn đã đánh giá chuyến đi này rồi.";
                return RedirectToAction("ChiTiet", "Profile", new { id = donDatLich.MaHDV });
            }

            var danhGiaMoi = new DANHGIA
            {
                MaDatLich = maDatLich,
                SoSao = soSao,
                NoiDung = noiDung
            };

            _context.DanhGias.Add(danhGiaMoi);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Cảm ơn bạn đã gửi đánh giá!";
            // Chuyển hướng về trang profile của HDV đó
            return RedirectToAction("ChiTiet", "Profile", new { id = donDatLich.MaHDV });
        }
    }
}
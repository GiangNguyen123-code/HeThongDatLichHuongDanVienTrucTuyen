using HeThongDatLich.Data;
using HeThongDatLich.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace HeThongDatLich.Controllers.HDVControllers
{
    public class HoSoHDVController : Controller
    {
        private readonly AppDbContext _context;
        public HoSoHDVController(AppDbContext context) => _context = context;

        private bool IsHDVLoggedIn()
        {
            var role = HttpContext.Session.GetString("UserRole");
            return role == "HDV";
        }

        private int GetCurrentHDVId()
        {
            var userId = HttpContext.Session.GetString("UserId");
            return int.TryParse(userId, out int id) ? id : 0;
        }

        public IActionResult Profile()
        {
            if (!IsHDVLoggedIn()) return RedirectToAction("DangNhapTK", "TaiKhoan");
            int hdvId = GetCurrentHDVId();
            if (hdvId == 0) return RedirectToAction("DangNhapTK", "TaiKhoan");

            var hoSo = _context.HDVs
                .Include(h => h.NguoiDung)
                .Include(h => h.PhuongXa).ThenInclude(p => p.TinhThanh)
                .Include(h => h.NgonNgu)
                .FirstOrDefault(h => h.MaHDV == hdvId);
            if (hoSo == null) return NotFound("Không tìm thấy hồ sơ HDV");

            // Thống kê bổ sung
            int toursCompleted = _context.DonDatLichs.Count(d => d.MaHDV == hdvId && d.TrangThai == 3);
            decimal totalEarnings = _context.HoaDons
                .Where(h => h.DonDatLich.MaHDV == hdvId && h.TrangThaiTT == true)
                .Sum(h => (decimal?)h.TongTien) ?? 0;
            var ratings = _context.DanhGias.Where(dg => dg.DonDatLich.MaHDV == hdvId);
            double avgRating = ratings.Any() ? ratings.Average(r => r.SoSao) : 0;

            ViewBag.ToursCompleted = toursCompleted;
            ViewBag.TotalEarnings = totalEarnings;
            ViewBag.AvgRating = avgRating;

            return View("~/Views/HDV/Profile.cshtml", hoSo);
        }
    }
}
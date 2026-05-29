using HeThongDatLich.Data;
using HeThongDatLich.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;
using HeThongDatLich.Extensions; // [ĐÃ BỔ SUNG EXTENSION]

namespace HeThongDatLich.Controllers.HDVControllers
{
    public class LichLamController : Controller
    {
        private readonly AppDbContext _context;
        public LichLamController(AppDbContext context) => _context = context;

        // Kiểm tra đăng nhập và quyền HDV
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

        [HttpGet]
        public IActionResult GetEvents(DateTime start, DateTime end)
        {
            int hdvId = GetCurrentHDVId();
            if (hdvId == 0) return Json(new List<object>());

            // Lấy các đơn đã xác nhận (TrangThai = 2) trong khoảng thời gian
            var bookings = _context.DonDatLichs
                .Include(d => d.KhachHang)
                .Include(d => d.GoiTour)
                .Where(d => d.MaHDV == hdvId && d.TrangThai == 2 && d.NgayDat >= start && d.NgayDat <= end)
                .ToList();

            // SỬA TẠI ĐÂY: Thay đổi title để hiển thị Tên Tour lên ô lịch
            var events = bookings.Select(b => new
            {
                id = b.MaDatLich,
                // Ưu tiên hiển thị tên Tour, nếu thuê tự do theo ngày thì hiện "Thuê tự do" kèm tên khách
                title = (b.GoiTour != null ? b.GoiTour.TenTour : "Thuê tự do") + " (" + (b.KhachHang?.HoTen ?? "Khách") + ")",
                start = b.NgayDat.ToString("yyyy-MM-dd"),
                description = b.GoiTour?.TenTour ?? "Thuê tự do",
                khungGio = b.KhungGio
            });

            return Json(events);
        }

        public IActionResult LichLamHDV()
        {
            if (!IsHDVLoggedIn()) return RedirectToAction("DangNhapTK", "TaiKhoan");
            int hdvId = GetCurrentHDVId();
            if (hdvId == 0) return RedirectToAction("DangNhapTK", "TaiKhoan");

            // Lấy thông tin HDV
            var hdv = _context.HDVs.Include(h => h.NguoiDung).FirstOrDefault(h => h.MaHDV == hdvId);
            ViewBag.HoTen = hdv?.NguoiDung?.HoTen;

            // Thống kê nhanh tháng này
            DateTime startOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            decimal thuNhapThangNay = _context.HoaDons
                .Where(h => h.DonDatLich != null && h.DonDatLich.MaHDV == hdvId && h.TrangThaiTT == true && h.NgayThanhToan >= startOfMonth)
                .Sum(h => (decimal?)h.TongTien) ?? 0;
            ViewBag.ThuNhapThang = thuNhapThangNay;

            DateTime startLast = startOfMonth.AddMonths(-1);
            DateTime endLast = startLast.AddMonths(1).AddDays(-1);
            decimal thuNhapThangTruoc = _context.HoaDons
                .Where(h => h.DonDatLich != null && h.DonDatLich.MaHDV == hdvId && h.TrangThaiTT == true && h.NgayThanhToan >= startLast && h.NgayThanhToan <= endLast)
                .Sum(h => (decimal?)h.TongTien) ?? 0;
            ViewBag.PhanTramTang = thuNhapThangTruoc == 0 ? 0 : (thuNhapThangNay - thuNhapThangTruoc) / thuNhapThangTruoc * 100;

            // Đánh giá trung bình
            var danhGias = _context.DanhGias.Where(dg => dg.DonDatLich.MaHDV == hdvId).ToList();
            double avgRating = danhGias.Any() ? danhGias.Average(dg => dg.SoSao) : 0;
            ViewBag.AvgRating = avgRating.ToString("F2");
            ViewBag.SoDanhGia = danhGias.Count;

            // Booking mới chờ xác nhận (Trạng thái = 1)
            ViewBag.BookingsMoi = _context.DonDatLichs
                .Include(d => d.KhachHang)
                .Include(d => d.GoiTour)
                .Where(d => d.MaHDV == hdvId && d.TrangThai == 1)
                .OrderByDescending(d => d.NgayDat)
                .Take(5)
                .ToList();

            // Lịch hôm nay đã chốt (Trạng thái = 2)
            DateTime today = DateTime.Today;
            ViewBag.LichHomNay = _context.DonDatLichs
                .Include(d => d.KhachHang)
                .Where(d => d.MaHDV == hdvId && d.NgayDat.Date == today && d.TrangThai == 2)
                .ToList();

            return View("~/Views/HDV/LichLamHDV.cshtml");
        }
    }
}
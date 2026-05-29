using HeThongDatLich.Data;
using HeThongDatLich.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using HeThongDatLich.Extensions; // [ĐÃ BỔ SUNG EXTENSION]

namespace HeThongDatLich.Controllers.HDVControllers
{
    public class DonDatLichController : Controller
    {
        private readonly AppDbContext _context;
        public DonDatLichController(AppDbContext context) => _context = context;

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

        // Danh sách booking có lọc và phân trang
        public IActionResult DanhSach(int? trangThai, string search, int page = 1, int pageSize = 10)
        {
            if (!IsHDVLoggedIn()) return RedirectToAction("DangNhapTK", "TaiKhoan");
            int hdvId = GetCurrentHDVId();
            if (hdvId == 0) return RedirectToAction("DangNhapTK", "TaiKhoan");

            var query = _context.DonDatLichs
                .Include(d => d.KhachHang)
                .Include(d => d.GoiTour)
                .Where(d => d.MaHDV == hdvId);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(d => d.KhachHang.HoTen.Contains(search) || d.KhachHang.SoDienThoai.Contains(search));
                ViewBag.Search = search;
            }

            if (trangThai.HasValue)
            {
                query = query.Where(d => d.TrangThai == trangThai.Value);
            }
            else
            {
                // THÊM SỐ 3 VÀO ĐÂY ĐỂ "TẤT CẢ" HIỂN THỊ CẢ ĐƠN ĐÃ HOÀN THÀNH
                query = query.Where(d => d.TrangThai == 1 || d.TrangThai == 2 || d.TrangThai == 3 || d.TrangThai == 4);
            }
            int total = query.Count();
            ViewBag.Total = total;

            var list = query.OrderByDescending(d => d.NgayDat)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

            return View("~/Views/HDV/DanhSach.cshtml", list);
        }

        // Xác nhận đơn
        [HttpPost]
        public IActionResult XacNhan(int maDatLich)
        {
            var booking = _context.DonDatLichs.Find(maDatLich);
            if (booking != null && booking.MaHDV == GetCurrentHDVId())
            {
                booking.TrangThai = 2; // 2: Đã xác nhận
                _context.SaveChanges();
                TempData["Success"] = "Đã xác nhận đơn đặt lịch.";
            }
            return RedirectToAction("DanhSach");
        }

        // Từ chối đơn (kèm lý do)
        [HttpPost]
        public IActionResult TuChoi(int maDatLich, string lyDo)
        {
            var booking = _context.DonDatLichs.Find(maDatLich);
            if (booking != null && booking.MaHDV == GetCurrentHDVId())
            {
                booking.TrangThai = 4; // 4: Từ chối / Hủy
                booking.GhiChu = string.IsNullOrEmpty(booking.GhiChu) ? $"Lý do hủy: {lyDo}" : $"{booking.GhiChu} | Lý do hủy: {lyDo}";
                _context.SaveChanges();
                TempData["Success"] = "Đã từ chối đơn.";
            }
            return RedirectToAction("DanhSach");
        }

        // Xem lịch sử công việc (đã hoàn thành)
        public IActionResult DanhGiaHDV()
        {
            if (!IsHDVLoggedIn()) return RedirectToAction("DangNhapTK", "TaiKhoan");
            int hdvId = GetCurrentHDVId();

            var completed = _context.DonDatLichs
                .Include(d => d.KhachHang)
                .Include(d => d.DanhGias)
                .Where(d => d.MaHDV == hdvId && d.TrangThai == 3)
                .OrderByDescending(d => d.NgayDat)
                .ToList();

            return View("~/Views/HDV/DanhGiaHDV.cshtml", completed);
        }

        [HttpPost]
        public IActionResult HoanThanh(int maDatLich)
        {
            var booking = _context.DonDatLichs.Find(maDatLich);
            if (booking != null && booking.MaHDV == GetCurrentHDVId())
            {
                booking.TrangThai = 3; // 3: Đã hoàn thành
                _context.SaveChanges();
                TempData["Success"] = "Chuyến đi đã hoàn tất! Khách hàng hiện có thể gửi đánh giá.";
            }
            return RedirectToAction("DanhSach");
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HeThongDatLich.Data;
using HeThongDatLich.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using HeThongDatLich.Extensions; // [ĐÃ BỔ SUNG] Extension xử lý tiền tệ

namespace HeThongDatLich.Controllers.AdminControllers
{
    public class ThongKeAdController : Controller
    {
        private readonly AppDbContext _context;

        public ThongKeAdController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int nam = 0)
        {
            int namChon = nam > 0 ? nam : DateTime.Now.Year;

            int tongBooking = _context.DonDatLichs?.Count() ?? 0;

            decimal tongDoanhThu = _context.HoaDons?
                .Where(hd => hd.TrangThaiTT == true)
                .Sum(hd => hd.TongTien) ?? 0;

            int tongNguoiDung = _context.NguoiDungs.Count();

            var doanhThuTheoThang = new List<decimal>();
            var bookingTheoThang = new List<int>();
            var mucTieuTheoThang = new List<decimal>();

            for (int thang = 1; thang <= 12; thang++)
            {
                decimal dt = _context.HoaDons?
                    .Where(hd => hd.NgayThanhToan != null
                              && hd.NgayThanhToan.Value.Month == thang
                              && hd.NgayThanhToan.Value.Year == namChon
                              && hd.TrangThaiTT == true)
                    .Sum(hd => hd.TongTien) ?? 0;

                int bk = _context.DonDatLichs?
                    .Count(d => d.NgayDat.Month == thang
                             && d.NgayDat.Year == namChon) ?? 0;

                doanhThuTheoThang.Add(dt / 1_000_000m);
                bookingTheoThang.Add(bk);
                mucTieuTheoThang.Add(dt / 1_000_000m * 1.15m);
            }

            int soDonXacNhan = _context.DonDatLichs?.Count(d => d.TrangThai == 1) ?? 0;
            int soDonHoanThanh = _context.DonDatLichs?.Count(d => d.TrangThai == 2) ?? 0;
            int soDonChoDuyet = _context.DonDatLichs?.Count(d => d.TrangThai == 0) ?? 0;
            int soDonHuy = _context.DonDatLichs?.Count(d => d.TrangThai == 3) ?? 0;

            var topTours = _context.DonDatLichs
            .Include(d => d.HoaDon)
            .Where(d => d.NgayDat.Year == namChon)
            .GroupBy(d => d.MaTour)
            .Select(g => new TopTourItem
            {
                MaTour = (int)(g.Key ?? 0),
                TenTour = "Tour #" + g.Key,
                SoBooking = g.Count(),
                DoanhThu = g.Sum(d => d.HoaDon != null ? d.HoaDon.TongTien : 0) / 1_000_000m
            })
            .OrderByDescending(t => t.DoanhThu)
            .Take(6)
            .ToList() ?? new List<TopTourItem>();

            var topHDVs = _context.HDVs
            .Select(h => new HdvPerformanceItem
            {
                MaHDV = h.MaHDV,
                HoTen = h.NguoiDung.HoTen,
                LoaiHDV = "Nội địa",
                DiaDiem = h.PhuongXa != null ? h.PhuongXa.TenPhuongXa : "Chưa cập nhật",
                SoTour = h.DonDatLichs.Count(d => d.NgayDat.Year == namChon),
                DanhGia = 5.0
            })
            .OrderByDescending(h => h.SoTour)
            .Take(5)
            .ToList();

            int maxTour = topHDVs.Any() ? topHDVs.Max(h => h.SoTour) : 1;
            topHDVs.ForEach(h => h.MaxTour = maxTour);

            var model = new ThongKeViewModel
            {
                TongBooking = tongBooking,
                TongDoanhThu = tongDoanhThu,
                TongNguoiDung = tongNguoiDung,
                TourDangChay = 47,
                DanhGiaTrungBinh = 4.7,

                TyLeBookingThayDoi = 12.4,
                TyLeDoanhThuThayDoi = 18.0,
                TyLeNguoiDungThayDoi = 5.1,

                DoanhThuTheoThang = doanhThuTheoThang,
                BookingTheoThang = bookingTheoThang,
                MucTieuTheoThang = mucTieuTheoThang,

                SoDonXacNhan = soDonXacNhan,
                SoDonHoanThanh = soDonHoanThanh,
                SoDonChoDuyet = soDonChoDuyet,
                SoDonHuy = soDonHuy,

                TopTours = topTours,
                TopHDVs = topHDVs,

                // [ĐÃ ÁP DỤNG EXTENSION]: Format thẳng số tiền vào chuỗi bằng .ToVnd()
                RecentActivities = new List<ActivityItem>
                {
                    new() { LoaiIcon="blue",  Icon="ti-ticket",        NoiDung="Booking #2481 vừa được xác nhận",       ThoiGian="2 phút trước"  },
                    new() { LoaiIcon="green", Icon="ti-user-plus",     NoiDung="Người dùng mới Hoa Nguyễn đăng ký",     ThoiGian="15 phút trước" },
                    new() { LoaiIcon="amber", Icon="ti-id-badge",      NoiDung="HDV Vũ Đức nộp hồ sơ chờ duyệt",       ThoiGian="32 phút trước" },
                    new() { LoaiIcon="green", Icon="ti-currency-dollar",NoiDung=$"Thanh toán {(2400000m).ToVnd()} thành công",    ThoiGian="1 giờ trước"   },
                    new() { LoaiIcon="red",   Icon="ti-x",             NoiDung="Booking #2479 bị hủy bởi khách hàng",   ThoiGian="2 giờ trước"   },
                    new() { LoaiIcon="blue",  Icon="ti-star",          NoiDung="Tour Đà Nẵng 3N2Đ nhận đánh giá 5★",   ThoiGian="3 giờ trước"   }
                }
            };

            return View("~/Views/Admin/ThongKe.cshtml", model);
        }

        [HttpGet]
        public JsonResult GetChartData(int nam, string kyTinh = "thang")
        {
            var doanhThu = new List<decimal>();
            var booking = new List<int>();

            for (int thang = 1; thang <= 12; thang++)
            {
                decimal dt = _context.HoaDons?
                    .Where(hd => hd.NgayThanhToan != null
                              && hd.NgayThanhToan.Value.Month == thang
                              && hd.NgayThanhToan.Value.Year == nam
                              && hd.TrangThaiTT == true)
                    .Sum(hd => hd.TongTien) ?? 0;

                int bk = _context.DonDatLichs?
                    .Count(d => d.NgayDat.Month == thang && d.NgayDat.Year == nam) ?? 0;

                doanhThu.Add(Math.Round(dt / 1_000_000m, 1));
                booking.Add(bk);
            }

            return Json(new { doanhThu, booking });
        }

        [HttpPost]
        public IActionResult XuatCSV()
        {
            var danhSachTour = _context.DonDatLichs
                .Include(d => d.GoiTour)
                .Include(d => d.HoaDon)
                .Where(d => d.NgayDat.Year == DateTime.Now.Year)
                .GroupBy(d => new { d.MaTour, d.GoiTour.TenTour })
                .Select(g => new {
                    TenTour = g.Key.TenTour ?? ("Tour ID: " + g.Key.MaTour),
                    SoBooking = g.Count(),
                    DoanhThu = g.Sum(d => d.HoaDon != null ? d.HoaDon.TongTien : 0)
                }).ToList();

            var builder = new System.Text.StringBuilder();
            builder.AppendLine("\uFEFFTen Tour,So Booking,Doanh Thu (VND)");

            foreach (var tour in danhSachTour)
            {
                // [ĐÃ ÁP DỤNG EXTENSION]: Format doanh thu sang dạng 1,000,000 VNĐ. 
                // Có bọc thêm \" để tránh lỗi phân cột vì trong chuỗi ToVnd() có chứa dấu phẩy.
                builder.AppendLine($"\"{tour.TenTour}\",{tour.SoBooking},\"{tour.DoanhThu.ToVnd()}\"");
            }

            return File(System.Text.Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "BaoCaoChiTietTour.csv");
        }
    }
}
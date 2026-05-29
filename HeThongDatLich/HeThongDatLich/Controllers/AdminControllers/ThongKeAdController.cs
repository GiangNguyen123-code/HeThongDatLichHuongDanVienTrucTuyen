using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HeThongDatLich.Data;
using HeThongDatLich.Models;
using System;
using System.Linq;
using System.Collections.Generic;

namespace HeThongDatLich.Controllers.AdminControllers
{
    public class ThongKeAdController : Controller
    {
        private readonly AppDbContext _context;

        public ThongKeAdController(AppDbContext context)
        {
            _context = context;
        }

        // GET /ThongKeAd/Index  (hoặc /Admin/ThongKe tùy routing)
        public IActionResult Index(int nam = 0)
        {
            int namChon = nam > 0 ? nam : DateTime.Now.Year;

            // ── KPI cards ─────────────────────────────────────────────────────
            int tongBooking = _context.DonDatLichs?.Count() ?? 0;

            decimal tongDoanhThu = _context.HoaDons?
                .Where(hd => hd.TrangThaiTT == true)
                .Sum(hd => hd.TongTien) ?? 0;

            int tongNguoiDung = _context.NguoiDungs.Count();

            // ── Biểu đồ doanh thu + booking 12 tháng ─────────────────────────
            var doanhThuTheoThang = new List<decimal>();
            var bookingTheoThang  = new List<int>();
            var mucTieuTheoThang  = new List<decimal>();

            for (int thang = 1; thang <= 12; thang++)
            {
                decimal dt = _context.HoaDons?
                    .Where(hd => hd.NgayThanhToan != null
                              && hd.NgayThanhToan.Value.Month == thang
                              && hd.NgayThanhToan.Value.Year  == namChon
                              && hd.TrangThaiTT == true)
                    .Sum(hd => hd.TongTien) ?? 0;

                int bk = _context.DonDatLichs?
                    .Count(d => d.NgayDat.Month == thang
                             && d.NgayDat.Year  == namChon) ?? 0;

                doanhThuTheoThang.Add(dt / 1_000_000m);   // đổi sang triệu ₫ cho biểu đồ
                bookingTheoThang.Add(bk);
                mucTieuTheoThang.Add(dt / 1_000_000m * 1.15m); // mục tiêu = 115%
            }

            // ── Trạng thái đơn ────────────────────────────────────────────────
            // Điều chỉnh giá trị TrangThai cho khớp với enum/constant trong DB của bạn
            int soDonXacNhan   = _context.DonDatLichs?.Count(d => d.TrangThai == 1) ?? 0;
            int soDonHoanThanh = _context.DonDatLichs?.Count(d => d.TrangThai == 2) ?? 0;
            int soDonChoDuyet  = _context.DonDatLichs?.Count(d => d.TrangThai == 0) ?? 0;
            int soDonHuy       = _context.DonDatLichs?.Count(d => d.TrangThai == 3) ?? 0;

            // ── Top 6 tours theo doanh thu ─────────────────────────────────────
            // Sửa tên bảng/cột cho khớp với model thực tế của bạn
            var topTours = _context.DonDatLichs
            .Include(d => d.HoaDon) // Đã đổi thành HoaDons (tên thuộc tính mới)
            .Where(d => d.NgayDat.Year == namChon)
            .GroupBy(d => d.MaTour)
            .Select(g => new TopTourItem
            {
                    MaTour    = (int)(g.Key ?? 0),
                    TenTour = "Tour #" + g.Key,    
                    SoBooking = g.Count(),
                DoanhThu = g.Sum(d => d.HoaDon != null ? d.HoaDon.TongTien : 0) / 1_000_000m
            })
                .OrderByDescending(t => t.DoanhThu)
                .Take(6)
                .ToList() ?? new List<TopTourItem>();

            // ── Top 5 HDV theo số tour dẫn ────────────────────────────────────
            // Điều chỉnh bảng liên kết HDV – DonDatLich nếu khác tên
            var topHDVs = _context.HDVs
            .Select(h => new HdvPerformanceItem
            {
                MaHDV = h.MaHDV,
                HoTen = h.NguoiDung.HoTen, // Lấy qua quan hệ với bảng NguoiDungs

                // Thay vì h.HDV.LoaiHDV, dùng dữ liệu bạn có:
                LoaiHDV = "Nội địa", // Hoặc một logic khác nếu bạn có thêm bảng loại HDV

                // Lấy địa điểm từ bảng PhuongXa liên kết
                DiaDiem = h.PhuongXa != null ? h.PhuongXa.TenPhuongXa : "Chưa cập nhật",

                // Đếm số tour từ collection DonDatLichs đã có sẵn trong class HOSOHDV
                SoTour = h.DonDatLichs.Count(d => d.NgayDat.Year == namChon),

                DanhGia = 5.0 // Tạm thời để mặc định nếu bảng DanhGia chưa liên kết
            })
            .OrderByDescending(h => h.SoTour)
            .Take(5)
            .ToList();

            // MaxTour dùng để tính % thanh progress bar
            int maxTour = topHDVs.Any() ? topHDVs.Max(h => h.SoTour) : 1;
            topHDVs.ForEach(h => h.MaxTour = maxTour);

            // ── Đóng gói ViewModel ────────────────────────────────────────────
            var model = new ThongKeViewModel
            {
                TongBooking          = tongBooking,
                TongDoanhThu         = tongDoanhThu,
                TongNguoiDung        = tongNguoiDung,
                TourDangChay         = 47,   // TODO: thay bằng truy vấn thực tế
                DanhGiaTrungBinh     = 4.7,  // TODO: tính từ bảng DanhGia

                TyLeBookingThayDoi   = 12.4,
                TyLeDoanhThuThayDoi  = 18.0,
                TyLeNguoiDungThayDoi = 5.1,

                DoanhThuTheoThang    = doanhThuTheoThang,
                BookingTheoThang     = bookingTheoThang,
                MucTieuTheoThang     = mucTieuTheoThang,

                SoDonXacNhan         = soDonXacNhan,
                SoDonHoanThanh       = soDonHoanThanh,
                SoDonChoDuyet        = soDonChoDuyet,
                SoDonHuy             = soDonHuy,

                TopTours             = topTours,
                TopHDVs              = topHDVs,

                // Activity feed: trong production lấy từ bảng log/event
                RecentActivities = new List<ActivityItem>
                {
                    new() { LoaiIcon="blue",  Icon="ti-ticket",        NoiDung="Booking #2481 vừa được xác nhận",       ThoiGian="2 phút trước"  },
                    new() { LoaiIcon="green", Icon="ti-user-plus",     NoiDung="Người dùng mới Hoa Nguyễn đăng ký",     ThoiGian="15 phút trước" },
                    new() { LoaiIcon="amber", Icon="ti-id-badge",      NoiDung="HDV Vũ Đức nộp hồ sơ chờ duyệt",       ThoiGian="32 phút trước" },
                    new() { LoaiIcon="green", Icon="ti-currency-dollar",NoiDung="Thanh toán 2,400,000 ₫ thành công",    ThoiGian="1 giờ trước"   },
                    new() { LoaiIcon="red",   Icon="ti-x",             NoiDung="Booking #2479 bị hủy bởi khách hàng",   ThoiGian="2 giờ trước"   },
                    new() { LoaiIcon="blue",  Icon="ti-star",          NoiDung="Tour Đà Nẵng 3N2Đ nhận đánh giá 5★",   ThoiGian="3 giờ trước"   }
                }
            };

            return View("~/Views/Admin/ThongKe.cshtml", model);
        }

        // AJAX endpoint – lấy lại dữ liệu biểu đồ khi đổi khoảng thời gian
        [HttpGet]
        public JsonResult GetChartData(int nam, string kyTinh = "thang")
        {
            // Trả về JSON để JavaScript cập nhật Chart.js mà không cần reload trang
            var doanhThu = new List<decimal>();
            var booking  = new List<int>();

            for (int thang = 1; thang <= 12; thang++)
            {
                decimal dt = _context.HoaDons?
                    .Where(hd => hd.NgayThanhToan != null
                              && hd.NgayThanhToan.Value.Month == thang
                              && hd.NgayThanhToan.Value.Year  == nam
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
            // Truy vấn lấy danh sách chi tiết các tour kèm doanh thu
            var danhSachTour = _context.DonDatLichs
                .Include(d => d.GoiTour) // Lấy thông tin tour
                .Include(d => d.HoaDon)
                .Where(d => d.NgayDat.Year == DateTime.Now.Year)
                .GroupBy(d => new { d.MaTour, d.GoiTour.TenTour }) // Nhóm theo Tour
                .Select(g => new {
                    TenTour = g.Key.TenTour ?? ("Tour ID: " + g.Key.MaTour),
                    SoBooking = g.Count(),
                    DoanhThu = g.Sum(d => d.HoaDon != null ? d.HoaDon.TongTien : 0)
                }).ToList();

            var builder = new System.Text.StringBuilder();
            // Thêm BOM để Excel đọc đúng tiếng Việt (UTF-8)
            builder.AppendLine("\uFEFFTen Tour,So Booking,Doanh Thu (VND)");

            foreach (var tour in danhSachTour)
            {
                // Định dạng dữ liệu cho chuẩn CSV
                builder.AppendLine($"\"{tour.TenTour}\",{tour.SoBooking},{tour.DoanhThu}");
            }

            return File(System.Text.Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "BaoCaoChiTietTour.csv");
        }
    }
}

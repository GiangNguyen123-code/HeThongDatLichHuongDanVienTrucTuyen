using System.Collections.Generic;

namespace HeThongDatLich.Models
{
    /// <summary>
    /// ViewModel dùng cho trang Thống kê Admin (/ThongKe/Index)
    /// </summary>
    public class ThongKeViewModel
    {
        // ── KPI cards ──────────────────────────────────────────────
        public int TongBooking { get; set; }
        public decimal TongDoanhThu { get; set; }
        public int TongNguoiDung { get; set; }
        public int TourDangChay { get; set; }
        public double DanhGiaTrungBinh { get; set; }

        // Tỷ lệ thay đổi so với kỳ trước (%)
        public double TyLeBookingThayDoi { get; set; }
        public double TyLeDoanhThuThayDoi { get; set; }
        public double TyLeNguoiDungThayDoi { get; set; }

        // ── Biểu đồ doanh thu / booking theo tháng ────────────────
        // Index 0 = T1, Index 11 = T12
        public List<decimal> DoanhThuTheoThang { get; set; } = new();
        public List<int>     BookingTheoThang  { get; set; } = new();
        public List<decimal> MucTieuTheoThang  { get; set; } = new();

        // ── Trạng thái đơn (doughnut) ─────────────────────────────
        public int SoDonXacNhan  { get; set; }
        public int SoDonHoanThanh { get; set; }
        public int SoDonChoDuyet { get; set; }
        public int SoDonHuy      { get; set; }

        // ── Top tours ─────────────────────────────────────────────
        public List<TopTourItem> TopTours { get; set; } = new();

        // ── HDV performance ───────────────────────────────────────
        public List<HdvPerformanceItem> TopHDVs { get; set; } = new();

        // ── Activity feed ─────────────────────────────────────────
        public List<ActivityItem> RecentActivities { get; set; } = new();
    }

    public class TopTourItem
    {
        public int    MaTour  { get; set; }
        public string TenTour { get; set; } = string.Empty;
        public int    SoBooking { get; set; }
        public decimal DoanhThu { get; set; }
    }

    public class HdvPerformanceItem
    {
        public int    MaHDV     { get; set; }
        public string HoTen     { get; set; } = string.Empty;
        public string LoaiHDV   { get; set; } = string.Empty;   // "Quốc tế" / "Nội địa"
        public string DiaDiem   { get; set; } = string.Empty;
        public int    SoTour    { get; set; }
        public double DanhGia   { get; set; }
        public int    MaxTour   { get; set; }   // dùng để tính % thanh bar
    }

    public class ActivityItem
    {
        public string LoaiIcon { get; set; } = "blue"; // blue | green | amber | red
        public string Icon     { get; set; } = "ti-bell";
        public string NoiDung  { get; set; } = string.Empty;
        public string ThoiGian { get; set; } = string.Empty;
    }
}

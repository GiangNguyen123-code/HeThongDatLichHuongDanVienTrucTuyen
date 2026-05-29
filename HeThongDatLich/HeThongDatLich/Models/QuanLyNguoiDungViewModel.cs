using System;
using System.Collections.Generic;

namespace HeThongDatLich.Models
{
    // ViewModel chính cho trang Quản lý Người dùng
    public class QuanLyNguoiDungViewModel
    {
        // ── 3 thẻ số liệu trên cùng ─────────────────────────────
        public int TongNguoiDung { get; set; }
        public int TongHDV { get; set; }

        // ── Danh sách người dùng (1 trang) ──────────────────────
        public List<NguoiDungRow> DanhSachNguoiDung { get; set; } = new();

        // ── Phân trang ───────────────────────────────────────────
        public int TrangHienTai { get; set; } = 1;
        public int TongSoTrang  { get; set; } = 1;
        public int PageSize     { get; set; } = 10;

        // ── Bộ lọc đang áp dụng ─────────────────────────────────
        public string? TuKhoaTimKiem { get; set; }
        public string? VaiTroLocHien { get; set; }   // "HDV" | "KHACH" | "ADMIN" | null

        // ── Hoạt động hệ thống gần đây ──────────────────────────
        public List<HoatDongItem> HoatDongGanDay { get; set; } = new();
    }

    // Một dòng trong bảng người dùng
    public class NguoiDungRow
    {
        public int MaNguoiDung { get; set; } // Khớp với Controller
        public string HoTen { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? SoDienThoai { get; set; }
        public string? AnhDaiDien { get; set; }
        public bool TrangThaiKhoa { get; set; } // Khớp với Controller
        public bool LaHDV { get; set; }         // Khớp với Controller
        public string TenQuyen { get; set; } = "KhachHang"; // Khớp với Controller

        public string MatKhau { get; set; }

        public List<PHANQUYEN> PhanQuyens { get; set; } = new();
    }

    // Một mục hoạt động ở góc dưới bên trái
    public class HoatDongItem
    {
        // "KhoaTaiKhoan" | "DangKyHDV" | (tuỳ mở rộng)
        public string LoaiHoatDong { get; set; } = string.Empty;

        // Nội dung HTML cho phép <strong> … </strong>
        public string NoiDung { get; set; } = string.Empty;

        // VD: "2 phút trước", "45 phút trước"
        public string ThoiGian { get; set; } = string.Empty;
    }
}

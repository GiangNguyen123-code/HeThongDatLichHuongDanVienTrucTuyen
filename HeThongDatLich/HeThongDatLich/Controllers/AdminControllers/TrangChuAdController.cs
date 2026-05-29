using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HeThongDatLich.Data;
using HeThongDatLich.Models;
using System;
using System.Linq;
using System.Collections.Generic;

namespace HeThongDatLich.Controllers.AdminControllers
{
    // Đảm bảo đường dẫn Controller khớp với khai báo định tuyến
    public class TrangChuAdController : Controller
    {
        private readonly AppDbContext _context;

        // Tiêm AppDbContext vào để tương tác với Cơ sở dữ liệu
        public TrangChuAdController(AppDbContext context)
        {
            _context = context;
        }

        // ĐỔI TÊN: Thành TongQuanAdmin để khớp với RedirectToAction từ TaiKhoanController
        public IActionResult TongQuanAdmin()
        {
            // 1. Tính toán số liệu cho 4 thẻ thông tin đầu trang
            int tongNguoiDung = _context.NguoiDungs.Count();

            // Đếm số lượng HDV bằng cách đếm các bản ghi có MaQuyen == 3 trong bảng PhanQuyens
            int tongHDV = _context.PhanQuyens.Count(pq => pq.MaQuyen == 3);

            // Giả định bạn có bảng DONDATLICH hoặc HOADON (hãy đổi tên bảng cho đúng với DB thực tế)
            // Ví dụ này tính toán dựa trên các bảng giả định phổ biến:
            int tongBooking = _context.DonDatLichs?.Count() ?? 3850;

            // Tính tổng cột số tiền từ bảng hóa đơn (chỉ tính những hóa đơn đã thanh toán thành công)
            decimal tongDoanhThu = _context.HoaDons?
                .Where(hd => hd.TrangThaiTT == true )
                .Sum(hd => hd.TongTien) ?? 84200;

            // Đếm tài khoản bị khóa (TrangThaiKhoa = true)
            int soTaiKhoanKhoa = _context.NguoiDungs.Count(u => u.TrangThaiKhoa == true);
            // Đếm tài khoản chưa có quyền (không tồn tại trong bảng PhanQuyens)
            int soTaiKhoanChuaCoQuyen = _context.NguoiDungs
                .Count(u => !_context.PhanQuyens.Any(pq => pq.MaNguoiDung == u.MaNguoiDung));

            // 2. Lấy dữ liệu doanh thu 6 tháng gần nhất để vẽ biểu đồ (T4 đến T9 như trong ảnh)
            List<decimal> doanhThuBieuDo = new List<decimal>();
            int namHienTai = DateTime.Now.Year;

            for (int thang = 4; thang <= 9; thang++)
            {
                var doanhThuThang = _context.HoaDons?
                    .Where(hd => hd.NgayThanhToan != null
                              && hd.NgayThanhToan.Value.Month == thang
                              && hd.NgayThanhToan.Value.Year == namHienTai
                              && hd.TrangThaiTT == true)
                    .Sum(hd => hd.TongTien) ?? 0;

                doanhThuBieuDo.Add(doanhThuThang);
            }

            // 3. Tính toán số lượng Hồ sơ hướng dẫn viên 
            // Giả định bạn có bảng HOSOHDV chứa danh sách hồ sơ đăng ký làm HDV
            // Sửa TrangThai thành TrangThaiHoatDong và so sánh với giá trị số nguyên (ví dụ là 0)
            int soHoSoChoDuyet = _context.HDVs?.Count(hs => hs.TrangThaiHoatDong == 0) ?? 18;

            // 4. Đóng gói tất cả dữ liệu vào ViewModel
            var model = new ThongKeAdminViewModel
            {
                TongNguoiDung = tongNguoiDung,
                TongHDV = tongHDV,
                TongBooking = tongBooking,
                TongDoanhThu = tongDoanhThu,
                DoanhThuCacThang = doanhThuBieuDo,
                SoHosoChoDuyetHDV = soHoSoChoDuyet,
                SoTaiKhoanKhoa = soTaiKhoanKhoa,
                SoTaiKhoanChuaCoQuyen = soTaiKhoanChuaCoQuyen
            };
           

            // Truyền Model dữ liệu động ra View hiển thị
            return View("~/Views/Admin/TongQuanAdmin.cshtml", model);
        }

        // Action xử lý sự kiện bấm nút "Tải báo cáo (.pdf)" ở góc dưới
        public IActionResult TaiBaoCaoPdf()
        {
            // Sau này bạn có thể cài thư viện iTextSharp hoặc QuestPDF để xuất file thực tế tại đây
            // Hiện tại trả về một thông báo hoặc file rỗng để tránh lỗi hệ thống
            return Content("Tính năng xuất file PDF báo cáo đang được xây dựng.");
        }

        // Action xử lý sự kiện bấm nút "Xem danh sách chờ" của khối HDV
        public IActionResult XemDanhSachChoDuyet()
        {
            // Chuyển hướng sang trang quản lý danh sách hồ sơ HDV
            return RedirectToAction("Index", "QuanLyHoSoHDV");
        }
        public IActionResult XemDanhSachNguoiDung()
        {
            // Chuyển hướng đến Index của NguoiDungAdController
            // Lưu ý: Controller của bạn đang có [Route("admin/nguoi-dung")] 
            // nên dùng tên Controller và Action chính xác
            return RedirectToAction("Index", "NguoiDungAd");
        }
    }
}
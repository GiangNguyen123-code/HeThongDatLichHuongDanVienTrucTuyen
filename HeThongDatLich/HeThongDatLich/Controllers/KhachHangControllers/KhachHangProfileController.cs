using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using HeThongDatLich.Data;
using HeThongDatLich.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

using HeThongDatLich.Enums;

namespace HeThongDatLich.Controllers.KhachHangControllers
{
    [Route("khach-hang/profile")]
    public class KhachHangProfileController : Controller
    {
        private readonly AppDbContext _db;

        public KhachHangProfileController(AppDbContext db)
        {
            _db = db;
        }

        // =========================================================================================
        // GHI CHÚ: Hàm này sử dụng để lấy toàn bộ thông tin tài khoản và tính toán thống kê (tổng đơn, đơn hoàn thành) của Khách hàng.
        // LINQ sử dụng: FirstOrDefaultAsync (Lấy User), Count (Đếm số lượng phần tử thỏa mãn điều kiện).
        // =========================================================================================
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            string userIdString = HttpContext.Session.GetString("UserId");
            string userRole = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(userIdString) || (userRole != "2" && userRole != "KhachHang"))
            {
                return RedirectToAction("DangNhapTK", "TaiKhoan");
            }

            int maKhachHang = int.Parse(userIdString);

            var user = await _db.NguoiDungs
                .Include(u => u.PhanQuyens)
                    .ThenInclude(pq => pq.Quyen)
                .Include(u => u.DonDatLichs)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.MaNguoiDung == maKhachHang);

            if (user == null)
            {
                return NotFound("Không tìm thấy tài khoản khách hàng.");
            }

            ViewBag.TongDonDat = user.DonDatLichs.Count;

            // [ÁP DỤNG ENUM]: Đếm đơn trạng thái Hoàn thành (3)
            ViewBag.DonHoanThanh = user.DonDatLichs.Count(d => d.TrangThai == (int)TrangThaiDonDat.HoanThanh);

            return View("~/Views/KhachHang/ThongTinCaNhan.cshtml", user);
        }

        // =========================================================================================
        // GHI CHÚ: Hàm này sử dụng để lưu các thay đổi thông tin cá nhân cơ bản của Khách hàng (Họ tên, SĐT, Ảnh đại diện).
        // LINQ sử dụng: FirstOrDefaultAsync (Tìm đúng User đang đăng nhập theo ID).
        // =========================================================================================
        [HttpPost("cap-nhat")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatThongTin(string hoTen, string soDienThoai, string anhDaiDien)
        {
            string userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString)) return RedirectToAction("DangNhapTK", "TaiKhoan");

            int maNguoiDung = int.Parse(userIdString);

            var user = await _db.NguoiDungs
                .FirstOrDefaultAsync(u => u.MaNguoiDung == maNguoiDung);

            if (user == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin tài khoản.";
                return RedirectToAction(nameof(Index));
            }

            user.HoTen = hoTen?.Trim();
            user.SoDienThoai = soDienThoai?.Trim();
            user.AnhDaiDien = string.IsNullOrWhiteSpace(anhDaiDien) ? "Avatar_Text" : anhDaiDien.Trim();

            await _db.SaveChangesAsync();
            TempData["Success"] = "Cập nhật thông tin hồ sơ thành công!";
            return RedirectToAction(nameof(Index));
        }

        // =========================================================================================
        // GHI CHÚ: Hàm này sử dụng để kiểm tra xác thực mật khẩu cũ và tiến hành cập nhật mật khẩu mới cho Khách hàng.
        // LINQ sử dụng: FirstOrDefaultAsync (Tìm User trong database).
        // =========================================================================================
        [HttpPost("doi-mat-khau")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoiMatKhau(string matKhauCu, string matKhauMoi, string xacNhanMatKhau)
        {
            string userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString)) return RedirectToAction("DangNhapTK", "TaiKhoan");

            int maNguoiDung = int.Parse(userIdString);

            var user = await _db.NguoiDungs
                .FirstOrDefaultAsync(u => u.MaNguoiDung == maNguoiDung);

            if (user == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin tài khoản.";
                return RedirectToAction(nameof(Index));
            }

            // 1. Kiểm tra tính chính xác của mật khẩu cũ
            if (user.MatKhau != matKhauCu)
            {
                TempData["Error"] = "Mật khẩu hiện tại nhập vào không chính xác!";
                return RedirectToAction(nameof(Index));
            }

            // 2. Kiểm tra khớp mật khẩu mới (Server-side check phòng hờ bypass JS)
            if (matKhauMoi != xacNhanMatKhau)
            {
                TempData["Error"] = "Mật khẩu xác nhận mới không trùng khớp!";
                return RedirectToAction(nameof(Index));
            }

            // 3. Tiến hành cập nhật mật khẩu mới
            user.MatKhau = matKhauMoi;
            await _db.SaveChangesAsync();

            TempData["Success"] = "Thay đổi mật khẩu tài khoản thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using HeThongDatLich.Data;
using HeThongDatLich.Models;
using System.Threading.Tasks;
using System.Linq;
using HeThongDatLich.Extensions; // [ĐÃ BỔ SUNG] Khai báo sẵn Extension

namespace HeThongDatLich.Controllers.AdminControllers
{
    [Route("admin/profile")]
    public class ProfileAdController : Controller
    {
        private readonly AppDbContext _db;

        public ProfileAdController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            string userIdString = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("DangNhapTK", "TaiKhoan");
            }

            int maNguoiDung = int.Parse(userIdString);

            var user = await _db.NguoiDungs
                .Include(u => u.PhanQuyens)
                    .ThenInclude(pq => pq.Quyen)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.MaNguoiDung == maNguoiDung);

            if (user == null)
            {
                return NotFound("Không tìm thấy thông tin tài khoản.");
            }

            return View("~/Views/Admin/MyProfile.cshtml", user);
        }

        [HttpPost("cap-nhat")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhat(NGUOIDUNG model)
        {
            string userIdString = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("DangNhapTK", "TaiKhoan");
            }

            int maNguoiDung = int.Parse(userIdString);

            var user = await _db.NguoiDungs.FirstOrDefaultAsync(u => u.MaNguoiDung == maNguoiDung);

            if (user != null)
            {
                user.HoTen = model.HoTen?.Trim();
                user.SoDienThoai = model.SoDienThoai?.Trim();

                await _db.SaveChangesAsync();
                TempData["Success"] = "Cập nhật hồ sơ thành công!";
            }
            else
            {
                TempData["Error"] = "Lỗi: Không tìm thấy tài khoản để cập nhật.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost("doi-mat-khau")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoiMatKhau(string MatKhauCu, string MatKhauMoi, string XacNhanMatKhau)
        {
            string userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString)) return RedirectToAction("DangNhapTK", "TaiKhoan");

            int maNguoiDung = int.Parse(userIdString);
            var user = await _db.NguoiDungs.FirstOrDefaultAsync(u => u.MaNguoiDung == maNguoiDung);

            if (user == null)
            {
                TempData["Error"] = "Lỗi: Không tìm thấy tài khoản.";
                return RedirectToAction(nameof(Index));
            }

            if (user.MatKhau != MatKhauCu)
            {
                TempData["Error"] = "Thất bại: Mật khẩu hiện tại không chính xác!";
                return RedirectToAction(nameof(Index));
            }

            if (MatKhauMoi != XacNhanMatKhau)
            {
                TempData["Error"] = "Thất bại: Mật khẩu xác nhận không khớp!";
                return RedirectToAction(nameof(Index));
            }

            user.MatKhau = MatKhauMoi;
            await _db.SaveChangesAsync();

            TempData["Success"] = "Đổi mật khẩu thành công! Hãy ghi nhớ mật khẩu mới của bạn.";
            return RedirectToAction(nameof(Index));
        }
    }
}
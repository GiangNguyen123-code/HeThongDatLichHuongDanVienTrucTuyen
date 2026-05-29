using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using HeThongDatLich.Data;
using HeThongDatLich.Models;
using System.Threading.Tasks;
using System.Linq;

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

        // GET: /admin/profile
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            // SỬA LỖI 2: Lấy đúng Key "UserId" dạng String mà TaiKhoanController đã lưu
            string userIdString = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdString))
            {
                // SỬA LỖI 1: Gọi đúng tên Action là "DangNhapTK"
                return RedirectToAction("DangNhapTK", "TaiKhoan");
            }

            // Chuyển chuỗi thành số nguyên
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

        // POST: /admin/profile/cap-nhat
        [HttpPost("cap-nhat")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhat(NGUOIDUNG model)
        {
            // SỬA LỖI TƯƠNG TỰ Ở HÀM POST
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

            // 1. Kiểm tra mật khẩu cũ có nhập đúng không
            if (user.MatKhau != MatKhauCu)
            {
                TempData["Error"] = "Thất bại: Mật khẩu hiện tại không chính xác!";
                return RedirectToAction(nameof(Index));
            }

            // 2. Kiểm tra an toàn thêm 1 lần nữa ở Server (phòng hờ bị bypass JS)
            if (MatKhauMoi != XacNhanMatKhau)
            {
                TempData["Error"] = "Thất bại: Mật khẩu xác nhận không khớp!";
                return RedirectToAction(nameof(Index));
            }

            // 3. Cập nhật và lưu
            user.MatKhau = MatKhauMoi;
            await _db.SaveChangesAsync();

            TempData["Success"] = "Đổi mật khẩu thành công! Hãy ghi nhớ mật khẩu mới của bạn.";
            return RedirectToAction(nameof(Index));
        }
    }
}
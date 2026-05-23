using HeThongDatLich.Data;
using HeThongDatLich.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeThongDatLich.Controllers.KhachHangControllers
{
    public class DatLichController : Controller
    {
        private readonly AppDbContext _context;

        public DatLichController(AppDbContext context)
        {
            _context = context;
        }

        // Đóng vai trò là một API nhỏ phục vụ cho giao diện đặt lịch
        [HttpPost]
        public async Task<IActionResult> KiemTraVoucher(string tenVoucher, decimal tongTienTamTinh)
        {
            if (string.IsNullOrEmpty(tenVoucher))
            {
                return Json(new { success = false, message = "Vui lòng nhập mã giảm giá." });
            }

            var voucher = await _context.MaGiamGias
                .FirstOrDefaultAsync(v => v.TenVoucher == tenVoucher
                                       && v.NgayHetHan >= DateTime.Now
                                       && v.SoLuong > 0);

            if (voucher == null)
            {
                return Json(new { success = false, message = "Mã giảm giá không hợp lệ hoặc đã hết hạn." });
            }

            decimal soTienGiam = (tongTienTamTinh * voucher.PhanTramGiam) / 100;
            if (soTienGiam > voucher.GiamToiDa)
            {
                soTienGiam = voucher.GiamToiDa;
            }

            decimal tongTienCuoiCung = tongTienTamTinh - soTienGiam;

            return Json(new
            {
                success = true,
                maVoucher = voucher.MaVoucher,
                soTienGiam = soTienGiam,
                tongTienMoi = tongTienCuoiCung,
                message = "Áp dụng mã giảm giá thành công!"
            });
        }

        // Hàm xử lý khi bấm nút "Xác nhận đặt lịch" cuối cùng
        [HttpPost]
        public async Task<IActionResult> LuuDonDatLich(DONDATLICH model)
        {
            // Logic lưu DONDATLICH và HOADON kế thừa từ phần trước...
            // Bạn có thể gọi trực tiếp logic tính toán tại đây để lưu vào database
            return RedirectToAction("Index", "Home");
        }
    }
}
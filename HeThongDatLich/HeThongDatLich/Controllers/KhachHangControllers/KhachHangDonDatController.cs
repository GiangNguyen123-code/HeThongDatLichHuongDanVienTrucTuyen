using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using HeThongDatLich.Data;
using HeThongDatLich.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HeThongDatLich.Controllers.KhachHangControllers
{
    public class KhachHangDonDatController : Controller
    {
        private readonly AppDbContext _context;
        public KhachHangDonDatController(AppDbContext context) { _context = context; }

        // API Chức năng 3.1.8: Kiểm tra Voucher (Không trả về View nên không cần sửa đường dẫn)
        [HttpGet]
        public async Task<IActionResult> KiemTraVoucher(string code)
        {
            var voucher = await _context.MaGiamGias.FirstOrDefaultAsync(v => v.TenVoucher == code.ToUpper());
            if (voucher == null) return Json(new { success = false, message = "Mã không hợp lệ." });
            if (voucher.NgayHetHan < DateTime.Now || voucher.SoLuong <= 0) return Json(new { success = false, message = "Mã hết hạn hoặc hết lượt." });
            return Json(new { success = true, discount = voucher.GiamToiDa });
        }

        // Chức năng 3.1.4: Tiếp nhận đơn đặt lịch (Sau khi xử lý xong dùng Redirect, không trả về View trực tiếp)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TaoDonDatLich(int MaHDV, DateTime NgayDat, string KhungGio, int? MaTour, string GhiChu, string VoucherCode, int MaPTTT)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("DangNhapTK", "TaiKhoan");
            int maKhachHang = int.Parse(userIdStr);

            // Kiểm tra bận lịch của HDV
            bool isConflict = await _context.DonDatLichs.AnyAsync(d => d.MaHDV == MaHDV && d.NgayDat.Date == NgayDat.Date && d.TrangThai != 4);
            if (isConflict)
            {
                TempData["ErrorMessage"] = "Hướng dẫn viên đã bận lịch vào ngày này rồi.";
                return RedirectToAction("ChiTietHDV", "KhachHangHome", new { id = MaHDV });
            }

            decimal tongTien = 0;
            if (MaTour.HasValue)
            {
                var tour = await _context.GoiTours.FindAsync(MaTour.Value);
                if (tour != null) tongTien = tour.GiaTien;
            }
            else
            {
                var hdv = await _context.HDVs.FindAsync(MaHDV);
                if (hdv != null) tongTien = hdv.GiaThue;
            }

            int? maVoucherDuocApDung = null;
            decimal soTienGiam = 0;
            if (!string.IsNullOrEmpty(VoucherCode))
            {
                var voucher = await _context.MaGiamGias.FirstOrDefaultAsync(v => v.TenVoucher == VoucherCode.ToUpper() && v.SoLuong > 0 && v.NgayHetHan >= DateTime.Now);
                if (voucher != null)
                {
                    maVoucherDuocApDung = voucher.MaVoucher;
                    soTienGiam = voucher.GiamToiDa;
                    tongTien = Math.Max(0, tongTien - soTienGiam);
                    voucher.SoLuong -= 1;
                }
            }

            var donMoi = new DONDATLICH { MaKhachHang = maKhachHang, MaHDV = MaHDV, MaTour = MaTour, MaVoucher = maVoucherDuocApDung, NgayDat = NgayDat, KhungGio = KhungGio, GhiChu = GhiChu, TrangThai = 1 };
            _context.DonDatLichs.Add(donMoi);
            await _context.SaveChangesAsync();

            var hoaDonMoi = new HOADON { MaDatLich = donMoi.MaDatLich, TongTien = tongTien, SoTienGiam = soTienGiam, TrangThaiTT = false, MaPTTT = MaPTTT };
            _context.HoaDons.Add(hoaDonMoi);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đặt lịch thành công!";
            return RedirectToAction("LichSu");
        }

        // Chức năng 3.1.5: Xem lịch sử danh sách đơn đặt lịch
        public async Task<IActionResult> LichSu()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("DangNhapTK", "TaiKhoan");
            int maKhachHang = int.Parse(userIdStr);

            var donHangs = await _context.DonDatLichs
                .Include(d => d.KhachHang) // <-- BỔ SUNG DÒNG NÀY ĐỂ LẤY TÊN KHÁCH HÀNG
                .Include(d => d.HDV).ThenInclude(h => h.NguoiDung)
                .Include(d => d.GoiTour)
                .Include(d => d.HoaDon).ThenInclude(hd => hd.PhuongThucThanhToan)
                .Include(d => d.DanhGias)
                .Where(d => d.MaKhachHang == maKhachHang)
                .OrderByDescending(d => d.NgayDat)
                .ToListAsync();

            return View("~/Views/KhachHang/LichSu.cshtml", donHangs);
        }

        // Chức năng 3.1.7: Gửi đánh giá phản hồi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuiDanhGia(int MaDatLich, int SoSao, string NoiDung)
        {
            var donHang = await _context.DonDatLichs.FindAsync(MaDatLich);
            if (donHang == null || donHang.TrangThai != 3) return RedirectToAction("LichSu");

            var danhGiaMoi = new DANHGIA { MaDatLich = MaDatLich, SoSao = SoSao, NoiDung = NoiDung };
            _context.DanhGias.Add(danhGiaMoi);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cảm ơn bạn đã gửi đánh giá!";
            return RedirectToAction("LichSu");
        }
    }
}
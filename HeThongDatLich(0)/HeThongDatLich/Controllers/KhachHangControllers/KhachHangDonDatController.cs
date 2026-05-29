using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using HeThongDatLich.Data;
using HeThongDatLich.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

// THÊM CÁC DÒNG USING NÀY
using HeThongDatLich.Enums;
using HeThongDatLich.Validations;
using HeThongDatLich.Extensions; // [ĐÃ BỔ SUNG EXTENSION]

namespace HeThongDatLich.Controllers.KhachHangControllers
{
    public class KhachHangDonDatController : Controller
    {
        private readonly AppDbContext _context;
        public KhachHangDonDatController(AppDbContext context) { _context = context; }

        // =========================================================================================
        // GHI CHÚ: Hàm này sử dụng để kiểm tra tính hợp lệ của mã giảm giá (Voucher) khi khách hàng nhập vào.
        // LINQ sử dụng: FirstOrDefaultAsync để tìm mã giảm giá đầu tiên khớp với tên.
        // =========================================================================================
        [HttpGet]
        public async Task<IActionResult> KiemTraVoucher(string code)
        {
            var voucher = await _context.MaGiamGias
                .FirstOrDefaultAsync(v => v.TenVoucher == code.ToUpper());

            if (voucher == null)
                return Json(new { success = false, message = "Mã không hợp lệ." });

            if (voucher.NgayHetHan < DateTime.Now || voucher.SoLuong <= 0)
                return Json(new { success = false, message = "Mã hết hạn hoặc hết lượt." });

            // [ÁP DỤNG EXTENSION]: Trả về thêm chuỗi discountText đã format VNĐ cho màn hình hiển thị.
            // Biến discount (số thực) vẫn giữ nguyên để JavaScript có thể tính toán cộng trừ mà không bị lỗi.
            return Json(new { success = true, discount = voucher.GiamToiDa, discountText = voucher.GiamToiDa.ToVnd() });//extension
        }

        // =========================================================================================
        // GHI CHÚ: Hàm này xử lý logic tiếp nhận và tạo đơn đặt lịch mới từ khách hàng.
        // LINQ sử dụng: AnyAsync, FindAsync, FirstOrDefaultAsync.
        // =========================================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TaoDonDatLich(int MaHDV, DateTime NgayDat, string KhungGio, int? MaTour, string GhiChu, string VoucherCode, int MaPTTT)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
                return RedirectToAction("DangNhapTK", "TaiKhoan");
            int maKhachHang = int.Parse(userIdStr);

            // [ÁP DỤNG VALIDATION]: Kiểm tra ngày đặt lịch không được chọn trong quá khứ
            if (!DatLichValidation.IsValidNgayDat(NgayDat, out string errorDateMsg))
            {
                TempData["ErrorMessage"] = errorDateMsg;
                return RedirectToAction("ChiTietHDV", "KhachHangHome", new { id = MaHDV });
            }

            // Kiểm tra HDV có bận lịch không, loại trừ trạng thái đã hủy
            bool isConflict = await _context.DonDatLichs
                .AnyAsync(d => d.MaHDV == MaHDV &&
                               d.NgayDat.Date == NgayDat.Date &&
                               d.TrangThai != (int)TrangThaiDonDat.DaHuy);

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
                var voucher = await _context.MaGiamGias
                    .FirstOrDefaultAsync(v => v.TenVoucher == VoucherCode.ToUpper() &&
                                              v.SoLuong > 0 &&
                                              v.NgayHetHan >= DateTime.Now);
                if (voucher != null)
                {
                    maVoucherDuocApDung = voucher.MaVoucher;
                    soTienGiam = voucher.GiamToiDa;
                    tongTien = Math.Max(0, tongTien - soTienGiam);
                    voucher.SoLuong -= 1;
                }
            }

            var donMoi = new DONDATLICH { MaKhachHang = maKhachHang, MaHDV = MaHDV, MaTour = MaTour, MaVoucher = maVoucherDuocApDung, NgayDat = NgayDat, KhungGio = KhungGio, GhiChu = GhiChu, TrangThai = (int)TrangThaiDonDat.ChoXacNhan };
            _context.DonDatLichs.Add(donMoi);
            await _context.SaveChangesAsync();

            var hoaDonMoi = new HOADON { MaDatLich = donMoi.MaDatLich, TongTien = tongTien, SoTienGiam = soTienGiam, TrangThaiTT = false, MaPTTT = MaPTTT };
            _context.HoaDons.Add(hoaDonMoi);
            await _context.SaveChangesAsync();

            // [ÁP DỤNG EXTENSION]: Truyền thẳng số tiền đã format vào câu thông báo thành công cho sống động
            TempData["SuccessMessage"] = $"Đặt lịch thành công! Tổng tiền cần thanh toán là {tongTien.ToVnd()}.";
            return RedirectToAction("LichSu");
        }

        public async Task<IActionResult> LichSu()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("DangNhapTK", "TaiKhoan");
            int maKhachHang = int.Parse(userIdStr);

            var donHangs = await _context.DonDatLichs
                .Include(d => d.KhachHang)
                .Include(d => d.HDV)
                    .ThenInclude(h => h.NguoiDung)
                .Include(d => d.GoiTour)
                .Include(d => d.HoaDon)
                    .ThenInclude(hd => hd.PhuongThucThanhToan)
                .Include(d => d.DanhGias)
                .Where(d => d.MaKhachHang == maKhachHang)
                .OrderByDescending(d => d.NgayDat)
                .ToListAsync();

            return View("~/Views/KhachHang/LichSu.cshtml", donHangs);
        }

        // =========================================================================================
        // GHI CHÚ: Hàm này xử lý việc khách hàng gửi đánh giá (số sao, nội dung).
        // LINQ sử dụng: FindAsync.
        // =========================================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuiDanhGia(int MaDatLich, int SoSao, string NoiDung)
        {
            if (!DanhGiaValidation.IsValidNoiDung(NoiDung, out string errorMsg))
            {
                TempData["ErrorMessage"] = errorMsg;
                return RedirectToAction("LichSu");
            }

            var donHang = await _context.DonDatLichs.FindAsync(MaDatLich);

            if (donHang == null || donHang.TrangThai != (int)TrangThaiDonDat.HoanThanh)
                return RedirectToAction("LichSu");

            var danhGiaMoi = new DANHGIA { MaDatLich = MaDatLich, SoSao = SoSao, NoiDung = NoiDung };
            _context.DanhGias.Add(danhGiaMoi);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cảm ơn bạn đã gửi đánh giá!";
            return RedirectToAction("LichSu");
        }
    }
}
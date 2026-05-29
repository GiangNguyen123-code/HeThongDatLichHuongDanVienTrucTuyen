using HeThongDatLich.Data;
using HeThongDatLich.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using HeThongDatLich.Extensions; // [ĐÃ BỔ SUNG EXTENSION]

namespace HeThongDatLich.Controllers.HDVControllers
{
    // Đổi Controller thành kế thừa HDVBaseController để tái sử dụng code phân quyền
    public class HoSoHDVController : HDVBaseController
    {
        private readonly AppDbContext _context;
        public HoSoHDVController(AppDbContext context) => _context = context;

        public IActionResult Profile()
        {
            var redirect = RedirectToLoginIfNotHDV();
            if (redirect != null) return redirect;

            int hdvId = GetCurrentHDVId();
            if (hdvId == 0) return RedirectToAction("DangNhapTK", "TaiKhoan");

            var hoSo = _context.HDVs
                .Include(h => h.NguoiDung)
                .Include(h => h.PhuongXa).ThenInclude(p => p.TinhThanh)
                .Include(h => h.NgonNgu)
                .FirstOrDefault(h => h.MaHDV == hdvId);

            if (hoSo == null) return NotFound("Không tìm thấy hồ sơ HDV");

            // Thống kê bổ sung
            ViewBag.ToursCompleted = _context.DonDatLichs.Count(d => d.MaHDV == hdvId && d.TrangThai == 3);
            ViewBag.TotalEarnings = _context.HoaDons
                .Where(h => h.DonDatLich.MaHDV == hdvId && h.TrangThaiTT == true)
                .Sum(h => (decimal?)h.TongTien) ?? 0;

            var ratings = _context.DanhGias.Where(dg => dg.DonDatLich.MaHDV == hdvId);
            ViewBag.AvgRating = ratings.Any() ? ratings.Average(r => r.SoSao) : 0;

            return View("~/Views/HDV/Profile.cshtml", hoSo);
        }

        // GET: Hiển thị form chỉnh sửa
        public IActionResult EditProfile()
        {
            var redirect = RedirectToLoginIfNotHDV();
            if (redirect != null) return redirect;

            int hdvId = GetCurrentHDVId();
            var hoSo = _context.HDVs
                .Include(h => h.NguoiDung)
                .FirstOrDefault(h => h.MaHDV == hdvId);

            if (hoSo == null) return NotFound();

            // Đổ dữ liệu ra DropDownList cho View (NgonNgu, PhuongXa...)
            ViewBag.MaNgonNgu = new SelectList(_context.NgonNgus, "MaNgonNgu", "TenNgonNgu", hoSo.MaNgonNgu);
            ViewBag.MaPhuongXa = new SelectList(_context.PhuongXas, "MaPhuongXa", "TenPhuongXa", hoSo.MaPhuongXa);

            return View("~/Views/HDV/EditProfile.cshtml", hoSo);
        }

        // POST: Xử lý cập nhật dữ liệu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProfile(HOSOHDV model, string HoTen, string SoDienThoai)
        {
            var redirect = RedirectToLoginIfNotHDV();
            if (redirect != null) return redirect;

            int hdvId = GetCurrentHDVId();
            if (hdvId != model.MaHDV) return BadRequest();

            foreach (var key in ModelState.Keys.ToList())
            {
                if (key.StartsWith("NguoiDung") || key.StartsWith("PhuongXa") || key.StartsWith("NgonNgu"))
                {
                    ModelState.Remove(key);
                }
            }

            // Kiểm tra dữ liệu bắt buộc thủ công cho ô Họ tên
            if (string.IsNullOrWhiteSpace(HoTen))
            {
                ModelState.AddModelError("HoTen", "Họ và tên không được để trống.");
            }

            if (ModelState.IsValid)
            {
                var hoSoDb = _context.HDVs
                    .Include(h => h.NguoiDung)
                    .FirstOrDefault(h => h.MaHDV == hdvId);

                if (hoSoDb == null) return NotFound();

                // Cập nhật bảng HOSOHDV
                hoSoDb.GioiThieu = model.GioiThieu;
                hoSoDb.KinhNghiem = model.KinhNghiem;
                hoSoDb.GiaThue = model.GiaThue;
                hoSoDb.MaNgonNgu = model.MaNgonNgu;
                hoSoDb.MaPhuongXa = model.MaPhuongXa;

                // Cập nhật bảng NGUOIDUNG
                if (hoSoDb.NguoiDung != null)
                {
                    hoSoDb.NguoiDung.HoTen = HoTen;
                    hoSoDb.NguoiDung.SoDienThoai = SoDienThoai;
                }

                _context.Update(hoSoDb);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Cập nhật hồ sơ thành công!";
                return RedirectToAction(nameof(Profile));
            }

            if (model.NguoiDung == null)
            {
                model.NguoiDung = new NGUOIDUNG();
            }
            model.NguoiDung.HoTen = HoTen;
            model.NguoiDung.SoDienThoai = SoDienThoai;

            // Load lại danh sách dropdown list
            ViewBag.MaNgonNgu = new SelectList(_context.NgonNgus, "MaNgonNgu", "TenNgonNgu", model.MaNgonNgu);
            ViewBag.MaPhuongXa = new SelectList(_context.PhuongXas, "MaPhuongXa", "TenPhuongXa", model.MaPhuongXa);

            return View("~/Views/HDV/EditProfile.cshtml", model);
        }
    }
}
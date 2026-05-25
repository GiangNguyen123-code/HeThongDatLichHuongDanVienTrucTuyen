using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HeThongDatLich.Data;
using HeThongDatLich.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeThongDatLich.Controllers.KhachHangControllers
{
    public class KhachHangHomeController : Controller
    {
        private readonly AppDbContext _context;
        public KhachHangHomeController(AppDbContext context) { _context = context; }

        // Chức năng 3.1.2: Tìm kiếm, lọc & Hiển thị danh sách HDV
        public async Task<IActionResult> Index(string tuKhoa, List<int> maTinh, List<int> maNgonNgu)
        {
            // LƯU Ý: Nếu _context.HDVs báo lỗi gạch đỏ, hãy sửa lại thành _context.HoSoHDVs cho khớp với AppDbContext của bạn nhé
            var query = _context.HDVs
                .Include(h => h.NguoiDung)
                .Include(h => h.NgonNgu)
                .Include(h => h.PhuongXa).ThenInclude(p => p.TinhThanh)
                .Include(h => h.DonDatLichs).ThenInclude(d => d.DanhGias)
                .Where(h => h.TrangThaiHoatDong == 1 && h.NguoiDung.TrangThaiKhoa != true);

            if (!string.IsNullOrEmpty(tuKhoa))
            {
                tuKhoa = tuKhoa.ToLower();
                query = query.Where(h => h.NguoiDung.HoTen.ToLower().Contains(tuKhoa) ||
                                         h.PhuongXa.TinhThanh.TenTinh.ToLower().Contains(tuKhoa));
            }

            if (maTinh != null && maTinh.Any())
            {
                query = query.Where(h => maTinh.Contains(h.PhuongXa.MaTinh));
            }

            if (maNgonNgu != null && maNgonNgu.Any())
            {
                query = query.Where(h => maNgonNgu.Contains(h.MaNgonNgu));
            }

            ViewBag.TuKhoa = tuKhoa;
            ViewBag.SelectedTinh = maTinh ?? new List<int>();
            ViewBag.SelectedNgonNgu = maNgonNgu ?? new List<int>();

            ViewBag.NgonNgus = await _context.NgonNgus.ToListAsync();
            ViewBag.TinhThanhs = await _context.TinhThanhs.ToListAsync();

            return View("~/Views/KhachHang/Index.cshtml", await query.ToListAsync());
        }

        // Chức năng 3.1.3: Xem chi tiết Profile HDV (Đã khôi phục lại)
        public async Task<IActionResult> ChiTietHDV(int id)
        {
            // Tương tự, đổi .HDVs thành .HoSoHDVs nếu bị gạch đỏ
            var hdv = await _context.HDVs
                .Include(h => h.NguoiDung)
                .Include(h => h.NgonNgu)
                .Include(h => h.PhuongXa).ThenInclude(px => px.TinhThanh)
                .Include(h => h.GoiTours)
                .Include(h => h.DonDatLichs).ThenInclude(d => d.KhachHang)
                .Include(h => h.DonDatLichs).ThenInclude(d => d.DanhGias)
                .FirstOrDefaultAsync(h => h.MaHDV == id);

            if (hdv == null) return NotFound();

            return View("~/Views/KhachHang/ChiTietHDV.cshtml", hdv);
        }
    }
}
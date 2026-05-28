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

        //=========================================================================================
        // GHI CHÚ: Hàm này là trang chủ của Khách hàng, sử dụng để lấy danh sách Hướng dẫn viên đang hoạt động,
        // đồng thời thực hiện logic Tìm kiếm theo Tên/Địa điểm và Lọc theo Tỉnh thành/Ngôn ngữ.
        // LINQ sử dụng: Include (nạp dữ liệu liên quan), Where (lọc điều kiện động), Contains (tìm chuỗi hoặc lọc danh sách mảng).
        // =========================================================================================
        public async Task<IActionResult> Index(string tuKhoa, List<int> maTinh, List<int> maNgonNgu)
        {
            var query = _context.HDVs
                .Include(h => h.NguoiDung)
                .Include(h => h.NgonNgu)
                .Include(h => h.PhuongXa)
                    .ThenInclude(p => p.TinhThanh)
                .Include(h => h.DonDatLichs)
                    .ThenInclude(d => d.DanhGias)
                .Where(h => h.TrangThaiHoatDong == 1 && h.NguoiDung.TrangThaiKhoa != true)
                .AsQueryable(); // Chuyển thành dạng Queryable để nối thêm các điều kiện Where bên dưới

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

        // =========================================================================================
        // GHI CHÚ: Hàm này dùng để xem thông tin hồ sơ chi tiết của một Hướng dẫn viên cụ thể (bao gồm các Tour, Đánh giá,...).
        // LINQ sử dụng: Include (nạp các bảng liên quan), FirstOrDefaultAsync (Tìm HDV khớp với ID truyền vào).
        // =========================================================================================
        public async Task<IActionResult> ChiTietHDV(int id)
        {
            var hdv = await _context.HDVs
                .Include(h => h.NguoiDung)
                .Include(h => h.NgonNgu)
                .Include(h => h.PhuongXa)
                    .ThenInclude(px => px.TinhThanh)
                .Include(h => h.GoiTours)
                .Include(h => h.DonDatLichs)
                    .ThenInclude(d => d.KhachHang)
                .Include(h => h.DonDatLichs)
                    .ThenInclude(d => d.DanhGias)
                .FirstOrDefaultAsync(h => h.MaHDV == id);

            if (hdv == null)
                return NotFound();

            return View("~/Views/KhachHang/ChiTietHDV.cshtml", hdv);
        }
    }
}
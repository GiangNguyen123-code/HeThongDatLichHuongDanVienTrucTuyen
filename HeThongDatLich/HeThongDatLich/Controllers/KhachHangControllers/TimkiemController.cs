using HeThongDatLich.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HeThongDatLich.Controllers.KhachHangControllers
{
    public class TimkiemController : Controller
    {
        private readonly AppDbContext _context;

        public TimkiemController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? maTinh, int? maNgonNgu, decimal? giaTu, decimal? giaDen)
        {
            var query = _context.HDVs
                .Include(h => h.NguoiDung)
                .Include(h => h.NgonNgu)
                .Include(h => h.PhuongXa).ThenInclude(p => p.TinhThanh)
                .AsQueryable();

            // Lọc HDV đang hoạt động (Giả sử 1 = Đang hoạt động)
            query = query.Where(h => h.TrangThaiHoatDong == 1);

            if (maTinh.HasValue && maTinh.Value > 0)
                query = query.Where(h => h.PhuongXa.MaTinh == maTinh.Value);

            if (maNgonNgu.HasValue && maNgonNgu.Value > 0)
                query = query.Where(h => h.MaNgonNgu == maNgonNgu.Value);

            if (giaTu.HasValue)
                query = query.Where(h => h.GiaThue >= giaTu.Value);

            if (giaDen.HasValue)
                query = query.Where(h => h.GiaThue <= giaDen.Value);

            ViewBag.TinhThanhs = new SelectList(await _context.TinhThanhs.ToListAsync(), "MaTinh", "TenTinh", maTinh);
            ViewBag.NgonNgus = new SelectList(await _context.NgonNgus.ToListAsync(), "MaNgonNgu", "TenNgonNgu", maNgonNgu);
            ViewBag.GiaTu = giaTu;
            ViewBag.GiaDen = giaDen;

            return View(await query.ToListAsync());
        }
    }
}
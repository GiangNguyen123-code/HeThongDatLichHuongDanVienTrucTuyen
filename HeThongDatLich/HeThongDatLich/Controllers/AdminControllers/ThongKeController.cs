using HeThongDatLich.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeThongDatLich.Controllers.AdminControllers
{
    public class ThongKeController : Controller
    {
        private readonly AppDbContext _context;

        public ThongKeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> DoanhThu(int? thang, int? nam)
        {
            int targetMonth = thang ?? DateTime.Now.Month;
            int targetYear = nam ?? DateTime.Now.Year;

            var query = _context.HoaDons
                .Include(h => h.DonDatLich)
                .ThenInclude(d => d.HDV)
                .ThenInclude(hdv => hdv.NguoiDung)
                .Where(h => h.TrangThaiTT == true
                         && h.NgayThanhToan.HasValue
                         && h.NgayThanhToan.Value.Month == targetMonth
                         && h.NgayThanhToan.Value.Year == targetYear);

            var thongKeDoanhThu = await query
                .GroupBy(h => new { h.DonDatLich.MaHDV, h.DonDatLich.HDV.NguoiDung.HoTen })
                .Select(group => new ThongKeViewModel
                {
                    MaHDV = group.Key.MaHDV,
                    TenHDV = group.Key.HoTen,
                    TongSoChuyen = group.Count(),
                    TongTienGiam = group.Sum(x => x.SoTienGiam),
                    TongDoanhThu = group.Sum(x => x.TongTien)
                })
                .OrderByDescending(x => x.TongDoanhThu)
                .ToListAsync();

            ViewBag.Thang = targetMonth;
            ViewBag.Nam = targetYear;
            ViewBag.TongDoanhThuHeThong = thongKeDoanhThu.Sum(x => x.TongDoanhThu);
            ViewBag.TongSoChuyenHeThong = thongKeDoanhThu.Sum(x => x.TongSoChuyen);

            return View(thongKeDoanhThu);
        }
    }

    public class ThongKeViewModel
    {
        public int MaHDV { get; set; }
        public string TenHDV { get; set; }
        public int TongSoChuyen { get; set; }
        public decimal TongTienGiam { get; set; }
        public decimal TongDoanhThu { get; set; }
    }
}
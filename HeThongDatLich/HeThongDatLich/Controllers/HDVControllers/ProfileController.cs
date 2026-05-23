using HeThongDatLich.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeThongDatLich.Controllers.KhachHangControllers
{
    public class ProfileController : Controller
    {
        private readonly AppDbContext _context;

        public ProfileController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> ChiTiet(int id)
        {
            var hdv = await _context.HDVs
                .Include(h => h.NguoiDung)
                .Include(h => h.NgonNgu)
                .Include(h => h.PhuongXa).ThenInclude(p => p.TinhThanh)
                .Include(h => h.GoiTours)
                .FirstOrDefaultAsync(h => h.MaHDV == id);

            if (hdv == null)
            {
                return NotFound();
            }

            return View(hdv);
        }
    }
}
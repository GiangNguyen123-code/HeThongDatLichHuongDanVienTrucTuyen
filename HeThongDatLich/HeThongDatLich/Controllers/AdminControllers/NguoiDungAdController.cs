using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeThongDatLich.Models;
using HeThongDatLich.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http; // Thêm thư viện để dùng Session
using Microsoft.AspNetCore.Mvc.Filters; // Thêm thư viện để dùng OnActionExecuting

namespace HeThongDatLich.Controllers.Admin
{
    [Route("admin/nguoi-dung")]
    public class NguoiDungAdController : Controller
    {
        private readonly AppDbContext _db;
        private const int PAGE_SIZE = 10;

        public NguoiDungAdController(AppDbContext db)
        {
            _db = db;
        }

        // =========================================================================
        // KIỂM TRA SESSION TRƯỚC KHI VÀO BẤT KỲ HÀM NÀO TRONG CONTROLLER NÀY
        // =========================================================================
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var role = HttpContext.Session.GetString("UserRole");
            // Nếu Session không tồn tại hoặc không phải là Admin (Quyền 1) thì đá về trang đăng nhập
            if (role != "1" && role != "Admin")
            {
                context.Result = RedirectToAction("DangNhapTK", "TaiKhoan");
                return;
            }
            base.OnActionExecuting(context);
        }

        // ═══════════════════════════════════════════════════════════
        // GET  /admin/nguoi-dung
        // Danh sách, tìm kiếm theo tên/email, lọc theo vai trò, phân trang
        // ═══════════════════════════════════════════════════════════
        [HttpGet("")]
        public async Task<IActionResult> Index(string? tuKhoa, string? vaiTro, int trang = 1)
        {
            // ── 1. Query gốc — nạp PhanQuyens → Quyen và HDV ─────
            var query = _db.NguoiDungs
                           .Include(u => u.PhanQuyens)
                               .ThenInclude(pq => pq.Quyen)
                           .Include(u => u.HDV)
                           .AsNoTracking();

            // ── 2. Lọc theo vai trò (TenQuyen: "Admin" | "HDV" | "KhachHang") ──
            if (!string.IsNullOrWhiteSpace(vaiTro))
            {
                query = query.Where(u =>
                    u.PhanQuyens.Any(pq => pq.Quyen.TenQuyen == vaiTro));
            }

            // ── 3. Tìm kiếm theo HoTen hoặc Email ────────────────
            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var kw = tuKhoa.Trim().ToLower();
                query = query.Where(u =>
                    u.HoTen.ToLower().Contains(kw) ||
                    u.Email.ToLower().Contains(kw));
            }

            // ── 4. Thẻ số liệu tổng quan ──────────────────────────
            // Tổng tất cả người dùng trong hệ thống
            var tongTatCa = await _db.NguoiDungs.CountAsync();

            // Đếm số người dùng có tồn tại quyền là "HDV"
            var tongHDV = await _db.NguoiDungs
                .CountAsync(u => u.PhanQuyens.Any(pq => pq.Quyen.TenQuyen == "HDV"));

            // ── 5. Phân trang ─────────────────────────────────────
            var tongBanGhi = await query.CountAsync();
            var tongTrang = (int)Math.Ceiling(tongBanGhi / (double)PAGE_SIZE);
            trang = Math.Max(1, Math.Min(trang, Math.Max(1, tongTrang)));

            var danhSach = await query
                .OrderBy(u => u.HoTen)
                .Skip((trang - 1) * PAGE_SIZE)
                .Take(PAGE_SIZE)
                .Select(u => new NguoiDungRow
                {
                    MaNguoiDung = u.MaNguoiDung,
                    HoTen = u.HoTen,
                    Email = u.Email,
                    SoDienThoai = u.SoDienThoai,
                    MatKhau = u.MatKhau,
                    AnhDaiDien = u.AnhDaiDien,
                    // TrangThaiKhoa: null hoặc false = hoạt động, true = bị khóa
                    TrangThaiKhoa = u.TrangThaiKhoa ?? false,
                    LaHDV = u.HDV != null,
                    // Lấy tên quyền đầu tiên để hiển thị badge
                    TenQuyen = u.PhanQuyens
                                     .Select(pq => pq.Quyen.TenQuyen)
                                     .FirstOrDefault() ?? "KhachHang",
                    PhanQuyens = u.PhanQuyens.ToList()
                })
                .ToListAsync();

            var viewModel = new QuanLyNguoiDungViewModel
            {
                TongNguoiDung = tongTatCa,
                TongHDV = tongHDV,
                DanhSachNguoiDung = danhSach, // danhSach đã được lấy ở bước 5 trong code của bạn
                TrangHienTai = trang,
                TongSoTrang = tongTrang,
                PageSize = PAGE_SIZE,
                TuKhoaTimKiem = tuKhoa,
                VaiTroLocHien = vaiTro
            };

            // Truyền viewModel sang View
            return View("~/Views/Admin/QuanLyNguoiDung.cshtml", viewModel);

        }

        [HttpPost("cap-nhat-quyen")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatQuyen(int maNguoiDung, List<int> maQuyens)
        {
            // 1. Xóa các quyền cũ của người dùng
            var oldRoles = await _db.PhanQuyens.Where(pq => pq.MaNguoiDung == maNguoiDung).ToListAsync();
            _db.PhanQuyens.RemoveRange(oldRoles);

            // 2. Thêm các quyền mới được chọn
            foreach (var maQuyen in maQuyens)
            {
                _db.PhanQuyens.Add(new PHANQUYEN { MaNguoiDung = maNguoiDung, MaQuyen = maQuyen });
            }

            await _db.SaveChangesAsync();
            TempData["Success"] = "Đã cập nhật quyền thành công.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("chinh-sua-toan-dien")]
        [ValidateAntiForgeryToken] // Rất quan trọng để tránh tấn công CSRF
        public async Task<IActionResult> ChinhSuaToanDien(int maNguoiDung, string hoTen, string email, string soDienThoai, string matKhauMoi, List<int> maQuyens)
        {
            // 1. Tìm user kèm quyền hiện tại
            var user = await _db.NguoiDungs.Include(u => u.PhanQuyens)
                                .FirstOrDefaultAsync(u => u.MaNguoiDung == maNguoiDung);

            if (user == null)
            {
                TempData["Error"] = "Không tìm thấy người dùng!";
                return RedirectToAction(nameof(Index));
            }

            // 2. Cập nhật thông tin
            user.HoTen = hoTen?.Trim();
            user.Email = email?.Trim().ToLower();
            user.SoDienThoai = soDienThoai?.Trim();

            if (!string.IsNullOrEmpty(matKhauMoi))
            {
                user.MatKhau = matKhauMoi;
            }

            // 3. Cập nhật quyền an toàn
            _db.PhanQuyens.RemoveRange(user.PhanQuyens); // Xóa quyền cũ

            // Kiểm tra xem danh sách quyền có dữ liệu không trước khi loop
            if (maQuyens != null && maQuyens.Any())
            {
                _db.PhanQuyens.RemoveRange(user.PhanQuyens); // Chỉ xóa nếu có thay đổi mới
                foreach (var qId in maQuyens)
                {
                    _db.PhanQuyens.Add(new PHANQUYEN { MaNguoiDung = user.MaNguoiDung, MaQuyen = qId });
                }
            }
            await _db.SaveChangesAsync();
            TempData["Success"] = "Cập nhật tài khoản thành công!";
            return RedirectToAction(nameof(Index));
        }

        // ═══════════════════════════════════════════════════════════
        // POST /admin/nguoi-dung/khoa/{id}
        // Toggle TrangThaiKhoa: null/false → true (khóa), true → false (mở)
        // ═══════════════════════════════════════════════════════════
        [HttpPost("khoa/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KhoaTaiKhoan(int id)
        {
            var user = await _db.NguoiDungs
                .FirstOrDefaultAsync(u => u.MaNguoiDung == id);

            if (user == null)
            {
                TempData["Error"] = "Không tìm thấy người dùng.";
                return RedirectToAction(nameof(Index));
            }

            // TrangThaiKhoa: true = đang khóa → mở; false/null → khóa
            bool dangKhoa = user.TrangThaiKhoa == true;
            user.TrangThaiKhoa = !dangKhoa;
            await _db.SaveChangesAsync();

            var hanhDong = dangKhoa ? "mở khóa" : "khóa";
            var loai = dangKhoa ? "MoKhoa" : "KhoaTaiKhoan";



            TempData["Success"] = $"Đã {hanhDong} tài khoản {user.HoTen}.";
            return RedirectToAction(nameof(Index));
        }

        // ═══════════════════════════════════════════════════════════
        // POST /admin/nguoi-dung/xoa/{id}
        // Xóa cứng: gỡ PHANQUYEN trước để tránh lỗi FK, sau đó xóa NGUOIDUNG
        // Khuyến nghị dùng KhoaTaiKhoan thay thế để giữ lịch sử
        // ═══════════════════════════════════════════════════════════
        [HttpPost("xoa/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Xoa(int id)
        {
            var user = await _db.NguoiDungs
                .Include(u => u.PhanQuyens)
                .FirstOrDefaultAsync(u => u.MaNguoiDung == id);

            if (user == null)
            {
                TempData["Error"] = "Không tìm thấy người dùng.";
                return RedirectToAction(nameof(Index));
            }

            // Xóa bản ghi phân quyền trước
            _db.PhanQuyens.RemoveRange(user.PhanQuyens);
            _db.NguoiDungs.Remove(user);
            await _db.SaveChangesAsync();



            TempData["Success"] = $"Đã xóa tài khoản {user.HoTen}.";
            return RedirectToAction(nameof(Index));
        }

        // ═══════════════════════════════════════════════════════════
        // GET  /admin/nguoi-dung/xuat-csv
        // Xuất file CSV UTF-8 có BOM (mở đúng tiếng Việt trong Excel)
        // Cột: MaNguoiDung, HoTen, Email, SoDienThoai, VaiTro, TrangThai, LaHDV
        // ═══════════════════════════════════════════════════════════
        [HttpGet("xuat-csv")]
        public async Task<IActionResult> XuatCSV()
        {
            var danhSach = await _db.NguoiDungs
                .Include(u => u.PhanQuyens)
                    .ThenInclude(pq => pq.Quyen)
                .Include(u => u.HDV)
                .AsNoTracking()
                .OrderBy(u => u.HoTen)
                .ToListAsync();

            var sb = new StringBuilder();
            sb.AppendLine("Mã ND,Họ Tên,Email,Số Điện Thoại,Vai Trò,Trạng Thái,Là HDV");

            foreach (var u in danhSach)
            {
                var tenQuyen = u.PhanQuyens.FirstOrDefault()?.Quyen?.TenQuyen ?? "KhachHang";
                var trangThai = u.TrangThaiKhoa == true ? "Đang khóa" : "Hoạt động";
                var laHDV = u.PhanQuyens.Any(pq => pq.Quyen.TenQuyen == "HDV") ? "Có" : "Không";

                sb.AppendLine(
                    $"{u.MaNguoiDung}," +
                    $"\"{u.HoTen}\"," +
                    $"{u.Email}," +
                    $"{u.SoDienThoai ?? ""}," +
                    $"\"{tenQuyen}\"," +
                    $"{trangThai}," +
                    $"{laHDV}"
                );
            }

            var bytes = Encoding.UTF8.GetPreamble()
                          .Concat(Encoding.UTF8.GetBytes(sb.ToString()))
                          .ToArray();
            var tenFile = $"nguoidung_{DateTime.Now:yyyyMMdd_HHmm}.csv";

            return File(bytes, "text/csv", tenFile);
        }

        // ═══════════════════════════════════════════════════════════
        // POST /admin/nguoi-dung/gui-email
        // ═══════════════════════════════════════════════════════════
        [HttpPost("gui-email")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuiEmailThongBao()
        {
            // TODO: Inject IEmailService → await _emailService.GuiThongBaoThangAsync();



            TempData["Success"] = "Đã gửi email thông báo thành công.";
            return RedirectToAction(nameof(Index));
        }

        // ═══════════════════════════════════════════════════════════
        // PRIVATE HELPERS
        // ═══════════════════════════════════════════════════════════


        private static string TinhThoiGianTuong(DateTime thoiDiem)
        {
            var delta = DateTime.Now - thoiDiem;
            if (delta.TotalMinutes < 1) return "Vừa xong";
            if (delta.TotalMinutes < 60) return $"{(int)delta.TotalMinutes} phút trước";
            if (delta.TotalHours < 24) return $"{(int)delta.TotalHours} giờ trước";
            if (delta.TotalDays < 7) return $"{(int)delta.TotalDays} ngày trước";
            return thoiDiem.ToString("dd/MM/yyyy");
        }
    }


    public class ChinhSuaNguoiDungDto
    {
        public int MaNguoiDung { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? SoDienThoai { get; set; }
        public string? MatKhau { get; set; } // Để trống nếu không đổi
        public List<int> MaQuyens { get; set; } = new List<int>(); // Danh sách quyền chọn
    }



    // ═══════════════════════════════════════════════════════════════
    // HoatDongItem — mục nhật ký hoạt động hệ thống
    // ═══════════════════════════════════════════════════════════════
    public class HoatDongItem
    {
        public string LoaiHoatDong { get; set; } = string.Empty;
        public string NoiDung { get; set; } = string.Empty;
        public string ThoiGian { get; set; } = string.Empty;
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using HeThongDatLich.Data;   // ← bỏ comment, đổi namespace cho đúng project
using HeThongDatLich.Models; // ← namespace chứa model NGUOIDUNG

namespace HeThongDatLich.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly AppDbContext _context;

        public TaiKhoanController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index() => RedirectToAction("DangNhapTK");

        #region ===== ĐĂNG NHẬP =====

        [HttpGet]
        public IActionResult DangNhapTK() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DangNhapTK(string tenDangNhap, string matKhau)
        {
            if (string.IsNullOrEmpty(tenDangNhap) || string.IsNullOrEmpty(matKhau))
            {
                ModelState.AddModelError("", "Vui lòng nhập đầy đủ thông tin.");
                return View();
            }

            // SỬA TẠI ĐÂY: Truy vấn kiểm tra Email thay vì HoTen để đồng bộ với Form đăng ký
            var nguoiDung = _context.NguoiDungs
                .FirstOrDefault(u => u.Email == tenDangNhap && u.MatKhau == matKhau);

            if (nguoiDung == null)
            {
                ModelState.AddModelError("", "Tên đăng nhập (Email) hoặc mật khẩu không chính xác.");
                return View();
            }

            // Kiểm tra nếu tài khoản bị khóa (Nếu trường TrangThaiKhoa == true)
            if (nguoiDung.TrangThaiKhoa == true)
            {
                ModelState.AddModelError("", "Tài khoản của bạn hiện đang bị khóa. Vui lòng liên hệ quản trị viên.");
                return View();
            }

            // Lưu thông tin vào Session để hiển thị lên thanh Navbar
            HttpContext.Session.SetString("UserName", nguoiDung.HoTen ?? "Người dùng");
            HttpContext.Session.SetString("UserId", nguoiDung.MaNguoiDung.ToString());

            // --- XỬ LÝ PHÂN QUYỀN ĐIỀU HƯỚNG ---
            // Tìm kiếm quyền của người dùng này từ bảng PhanQuyens
            var phanQuyen = _context.PhanQuyens
                .FirstOrDefault(pq => pq.MaNguoiDung == nguoiDung.MaNguoiDung);

            string userRole = "KhachHang"; // Mặc định nếu tài khoản mới đăng ký chưa được gán vai trò gì

            if (phanQuyen != null)
            {
                var quyen = _context.Quyens.FirstOrDefault(q => q.MaQuyen == phanQuyen.MaQuyen);
                if (quyen != null)
                {
                    userRole = quyen.TenQuyen; // Lấy tên quyền thực tế trong DB (Ví dụ: "Admin", "HDV")
                }
            }

            // Lưu vai trò quyền vào Session
            HttpContext.Session.SetString("UserRole", userRole);

            // Chuyển hướng khu vực tương ứng dựa trên quyền thực tế trong DB
            if (userRole == "Admin")
            {
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
            else if (userRole == "HDV")
            {
                return RedirectToAction("Index", "Home", new { area = "HDV" });
            }

            // Trả về trang chủ Client đối với Khách hàng
            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region ================= ĐĂNG KÝ =================

        // GET: /TaiKhoan/DangKyTK
        [HttpGet]
        public IActionResult DangKyTK()
        {
            return View();
        }

        // POST: /TaiKhoan/DangKyTK
        [HttpPost]
        [ValidateAntiForgeryToken] // Bảo mật chống tấn công giả mạo CSRF
        public IActionResult DangKyTK(string hoTen, string soDienThoai, string email, string matKhau, string nhapLaiMatKhau)
        {
            // 1. Kiểm tra dữ liệu đầu vào bắt buộc từ Form
            if (string.IsNullOrEmpty(hoTen) || string.IsNullOrEmpty(soDienThoai) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(matKhau))
            {
                ModelState.AddModelError("", "Vui lòng điền đầy đủ tất cả các trường thông tin.");
                return View();
            }

            // 2. Kiểm tra mật khẩu nhập lại có trùng khớp không
            if (matKhau != nhapLaiMatKhau)
            {
                ModelState.AddModelError("", "Mật khẩu xác nhận không trùng khớp.");
                return View();
            }

            // 3. Kiểm tra xem Email (Tài khoản đăng nhập) này đã tồn tại trong DB chưa
            var isEmailExist = _context.NguoiDungs.Any(u => u.Email == email);
            if (isEmailExist)
            {
                ModelState.AddModelError("", "Địa chỉ Email này đã được đăng ký tài khoản trước đó.");
                return View();
            }

            try
            {
                // 4. Khởi tạo đối tượng NGUOIDUNG mới tương ứng cấu hình bảng database của bạn
                var nguoiDungMoi = new NGUOIDUNG
                {
                    HoTen = hoTen,
                    SoDienThoai = soDienThoai,
                    Email = email,
                    MatKhau = matKhau,             // Lưu ý: Sau này bạn nên băm mật khẩu (Hash) để bảo mật tốt hơn nhé
                    AnhDaiDien = "Avatar_Text",    // Giá trị chuỗi mặc định phục vụ render avatar chữ lúc ban đầu
                    TrangThaiKhoa = false          // Tài khoản mới mặc định hoạt động bình thường
                };

                // 5. Thêm dữ liệu vào DbContext và lưu xuống SQL Server
                _context.NguoiDungs.Add(nguoiDungMoi);
                _context.SaveChanges();

                // 6. Đăng ký thành công -> Sử dụng TempData để gửi thông báo hiển thị ngắn gọn qua trang Đăng nhập
                TempData["SuccessMessage"] = "Đăng ký tài khoản thành công! Vui lòng đăng nhập.";

                // 7. Chuyển hướng người dùng sang trang Đăng nhập (Action DangNhapTK)
                return RedirectToAction("DangNhapTK");
            }
            catch (System.Exception ex)
            {
                // Xử lý lỗi ngoại lệ nếu quá trình lưu database gặp trục trặc (ví dụ: mất kết nối, lỗi kiểu dữ liệu...)
                ModelState.AddModelError("", "Đã có lỗi xảy ra trong quá trình lưu dữ liệu: " + ex.Message);
                return View();
            }
        }

        #endregion

        #region ===== ĐĂNG XUẤT =====

        [HttpGet]
        public IActionResult DangXuat()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("DangNhapTK");
        }

        #endregion
    }
}
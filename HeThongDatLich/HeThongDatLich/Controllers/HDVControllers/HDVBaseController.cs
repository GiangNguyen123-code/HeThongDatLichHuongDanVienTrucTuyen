using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace HeThongDatLich.Controllers.HDVControllers
{
    public abstract class HDVBaseController : Controller
    {
        protected bool IsHDVLoggedIn()
        {
            var role = HttpContext.Session.GetString("UserRole");
            return role == "HDV";
        }

        protected int GetCurrentHDVId()
        {
            var userId = HttpContext.Session.GetString("UserId");
            return int.TryParse(userId, out int id) ? id : 0;
        }

        protected IActionResult? RedirectToLoginIfNotHDV()
        {
            if (!IsHDVLoggedIn())
                return RedirectToAction("DangNhapTK", "TaiKhoan");
            return null;
        }
    }
}
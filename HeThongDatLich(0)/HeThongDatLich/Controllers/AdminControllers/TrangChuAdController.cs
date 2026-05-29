using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HeThongDatLich.Data;
using HeThongDatLich.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using HeThongDatLich.Extensions; // [ĐÃ BỔ SUNG] Khai báo Extension

namespace HeThongDatLich.Controllers.AdminControllers
{
    public class TrangChuAdController : Controller
    {
        private readonly AppDbContext _context;

        public TrangChuAdController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult TongQuanAdmin()
        {
            int tongNguoiDung = _context.NguoiDungs.Count();
            int tongHDV = _context.PhanQuyens.Count(pq => pq.MaQuyen == 3);
            int tongBooking = _context.DonDatLichs?.Count() ?? 3850;

            decimal tongDoanhThu = _context.HoaDons?
                .Where(hd => hd.TrangThaiTT == true)
                .Sum(hd => hd.TongTien) ?? 84200;

            int soTaiKhoanKhoa = _context.NguoiDungs.Count(u => u.TrangThaiKhoa == true);
            int soTaiKhoanChuaCoQuyen = _context.NguoiDungs
                .Count(u => !_context.PhanQuyens.Any(pq => pq.MaNguoiDung == u.MaNguoiDung));

            List<decimal> doanhThuBieuDo = new List<decimal>();
            int namHienTai = DateTime.Now.Year;

            for (int thang = 4; thang <= 9; thang++)
            {
                var doanhThuThang = _context.HoaDons?
                    .Where(hd => hd.NgayThanhToan != null
                              && hd.NgayThanhToan.Value.Month == thang
                              && hd.NgayThanhToan.Value.Year == namHienTai
                              && hd.TrangThaiTT == true)
                    .Sum(hd => hd.TongTien) ?? 0;

                doanhThuBieuDo.Add(doanhThuThang);
            }

            int soHoSoChoDuyet = _context.HDVs?.Count(hs => hs.TrangThaiHoatDong == 0) ?? 18;

            var model = new ThongKeAdminViewModel
            {
                TongNguoiDung = tongNguoiDung,
                TongHDV = tongHDV,
                TongBooking = tongBooking,
                TongDoanhThu = tongDoanhThu, // Giữ nguyên kiểu Decimal của logic cũ
                DoanhThuCacThang = doanhThuBieuDo,
                SoHosoChoDuyetHDV = soHoSoChoDuyet,
                SoTaiKhoanKhoa = soTaiKhoanKhoa,
                SoTaiKhoanChuaCoQuyen = soTaiKhoanChuaCoQuyen
            };

            ViewBag.TongDoanhThuFormatted = tongDoanhThu.ToVnd();

            return View("~/Views/Admin/TongQuanAdmin.cshtml", model);
        }

        public IActionResult TaiBaoCaoPdf()
        {
            return Content("Tính năng xuất file PDF báo cáo đang được xây dựng.");
        }

        public IActionResult XemDanhSachChoDuyet()
        {
            return RedirectToAction("Index", "QuanLyHoSoHDV");
        }

        public IActionResult XemDanhSachNguoiDung()
        {
            return RedirectToAction("Index", "NguoiDungAd");
        }
    }
}
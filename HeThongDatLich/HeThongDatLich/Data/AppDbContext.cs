using HeThongDatLich.Models;
using Microsoft.EntityFrameworkCore;
namespace HeThongDatLich.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<TINHTHANH> TinhThanhs { get; set; }
    public DbSet<PHUONGXA> PhuongXas { get; set; }
    public DbSet<QUYEN> Quyens { get; set; }
    public DbSet<PHANQUYEN> PhanQuyens { get; set; }
    public DbSet<NGUOIDUNG> NguoiDungs { get; set; }
    public DbSet<HOSOHDV> HDVs { get; set; }
    public DbSet<NGONNGU> NgonNgus { get; set; }
    public DbSet<GOITOUR> GoiTours { get; set; }
    public DbSet<MAGIAMGIA> MaGiamGias { get; set; }
    public DbSet<DONDATLICH> DonDatLichs { get; set; }
    public DbSet<DANHGIA> DanhGias { get; set; }
    public DbSet<PHUONGTHUCTHANHTOAN> PhuongThucThanhToans { get; set; }
    public DbSet<HOADON> HoaDons { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1. Cấu hình khóa kép cho bảng trung gian PhanQuyens
        modelBuilder.Entity<PHANQUYEN>()
            .HasKey(pq => new { pq.MaNguoiDung, pq.MaQuyen });

        // 2. Cấu hình mối quan hệ 1-1 giữa DonDatLichs và HoaDons qua trường MaDatLich
        modelBuilder.Entity<DONDATLICH>()
            .HasOne(d => d.HoaDon)
            .WithOne(h => h.DonDatLich)
            .HasForeignKey<HOADON>(h => h.MaDatLich)
            .OnDelete(DeleteBehavior.Restrict);

        // 3. Cấu hình mối quan hệ 1-1 giữa NguoiDungs và HDVs (MaNguoiDung FK trỏ tới MaHDV PK)
        modelBuilder.Entity<NGUOIDUNG>()
            .HasOne(n => n.HDV)
            .WithOne(h => h.NguoiDung)
            .HasForeignKey<NGUOIDUNG>(n => n.MaNguoiDung)
            .OnDelete(DeleteBehavior.Restrict);

        // 4. Bật chế độ Restrict cho các mối quan hệ có khả năng gây lỗi lặp Cascade (Multiple Cascade Paths)
        modelBuilder.Entity<DONDATLICH>()
            .HasOne(d => d.KhachHang)
            .WithMany(n => n.DonDatLichs)
            .HasForeignKey(d => d.MaKhachHang)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<DONDATLICH>()
            .HasOne(d => d.HDV)
            .WithMany(h => h.DonDatLichs)
            .HasForeignKey(d => d.MaHDV)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<GOITOUR>()
            .HasOne(g => g.HDV)
            .WithMany(h => h.GoiTours)
            .HasForeignKey(g => g.MaHDV)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
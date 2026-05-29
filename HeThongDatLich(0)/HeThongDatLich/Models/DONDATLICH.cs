using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeThongDatLich.Models
{
    [Table("DonDatLichs")]
    public class DONDATLICH
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaDatLich { get; set; }

        public int MaKhachHang { get; set; }
        public int MaHDV { get; set; }
        public int? MaVoucher { get; set; }
        public int? MaTour { get; set; }

        [Column(TypeName = "date")]
        public DateTime NgayDat { get; set; }

        [Required]
        [StringLength(50)]
        public string KhungGio { get; set; }

        // Sửa ở đây: Cho phép ghi chú trống
        [StringLength(500)]
        public string? GhiChu { get; set; }

        public int TrangThai { get; set; }

        [ForeignKey(nameof(MaKhachHang))]
        public virtual NGUOIDUNG KhachHang { get; set; }

        [ForeignKey(nameof(MaHDV))]
        public virtual HOSOHDV HDV { get; set; }

        [ForeignKey(nameof(MaVoucher))]
        public virtual MAGIAMGIA MaGiamGia { get; set; }

        [ForeignKey(nameof(MaTour))]
        public virtual GOITOUR GoiTour { get; set; }

        public virtual HOADON HoaDon { get; set; } 

        public virtual ICollection<DANHGIA> DanhGias { get; set; } = new List<DANHGIA>();
    }
}
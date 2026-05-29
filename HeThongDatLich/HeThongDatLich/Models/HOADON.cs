using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeThongDatLich.Models
{
    [Table("HoaDons")]
    public class HOADON
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaHoaDon { get; set; }

        public int MaDatLich { get; set; }

        [Column(TypeName = "decimal(12, 2)")]
        public decimal TongTien { get; set; }

        [Column(TypeName = "datetime2(7)")]
        public DateTime? NgayThanhToan { get; set; }

        public bool TrangThaiTT { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? SoTienGiam { get; set; }

        public int MaPTTT { get; set; }

        [ForeignKey(nameof(MaDatLich))]
        public virtual DONDATLICH DonDatLich { get; set; }

        [ForeignKey(nameof(MaPTTT))]
        public virtual PHUONGTHUCTHANHTOAN PhuongThucThanhToan { get; set; }
    }
}

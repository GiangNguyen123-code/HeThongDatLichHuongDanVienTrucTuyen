using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeThongDatLich.Models
{
    [Table("MaGiamGias")]
    public class MAGIAMGIA
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaVoucher { get; set; }

        [Required]
        [StringLength(100)]
        public string TenVoucher { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal PhanTramGiam { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal GiamToiDa { get; set; }

        [Column(TypeName = "datetime2(7)")]
        public DateTime NgayHetHan { get; set; }

        public int SoLuong { get; set; }

        public virtual ICollection<DONDATLICH> DonDatLichs { get; set; } = new List<DONDATLICH>();
    }
}

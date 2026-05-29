using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeThongDatLich.Models
{
    [Table("GoiTours")]
    public class GOITOUR
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaTour { get; set; }

        public int MaHDV { get; set; }

        [Required]
        [StringLength(200)]
        public string TenTour { get; set; }

        [StringLength(1000)]
        public string? MoTa { get; set; }

        [Column(TypeName = "decimal(12, 2)")]
        public decimal GiaTien { get; set; }

        [StringLength(50)]
        public string? ThoiGian { get; set; }

        [ForeignKey(nameof(MaHDV))]
        public virtual HOSOHDV HDV { get; set; }

        public virtual ICollection<DONDATLICH> DonDatLichs { get; set; } = new List<DONDATLICH>();
    }
}

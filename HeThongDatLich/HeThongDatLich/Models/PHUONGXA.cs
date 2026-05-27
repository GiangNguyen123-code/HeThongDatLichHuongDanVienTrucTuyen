using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeThongDatLich.Models
{
    [Table("PhuongXas")]
    public class PHUONGXA
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaPhuongXa { get; set; }

        [Required]
        [StringLength(100)]
        public string TenPhuongXa { get; set; }

        public int MaTinh { get; set; }

        [ForeignKey(nameof(MaTinh))]
        public virtual TINHTHANH TinhThanh { get; set; }

        public virtual ICollection<HOSOHDV> HDVs { get; set; } = new List<HOSOHDV>();
    }
}

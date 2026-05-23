using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeThongDatLich.Models
{
    [Table("PhuongThucThanhToans")]
    public class PHUONGTHUCTHANHTOAN
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MaPTTT { get; set; }

        [Required]
        [StringLength(100)]
        public string TenPhuongThuc { get; set; }

        [StringLength(200)]
        public string GhiChu { get; set; }

        public virtual ICollection<HOADON> HoaDons { get; set; } = new List<HOADON>();
    }
}

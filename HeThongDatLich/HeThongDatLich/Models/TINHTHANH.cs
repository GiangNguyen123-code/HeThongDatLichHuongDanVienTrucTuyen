using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeThongDatLich.Models
{
    [Table("TinhThanhs")] 
    public class TINHTHANH
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int MaTinh { get; set; }

        [Required]
        [StringLength(100)]
        public string TenTinh { get; set; }

        public virtual ICollection<PHUONGXA> PhuongXas { get; set; } = new List<PHUONGXA>();
    }
}

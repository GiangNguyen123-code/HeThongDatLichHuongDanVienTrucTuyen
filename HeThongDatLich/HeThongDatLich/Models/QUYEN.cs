using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeThongDatLich.Models
{
    [Table("Quyens")]
    public class QUYEN
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaQuyen { get; set; }

        [Required]
        [StringLength(50)]
        public string TenQuyen { get; set; }

        public virtual ICollection<PHANQUYEN> PhanQuyens { get; set; } = new List<PHANQUYEN>();
    }
}

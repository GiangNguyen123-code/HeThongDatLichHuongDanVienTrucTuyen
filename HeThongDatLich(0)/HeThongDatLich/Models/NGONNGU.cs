using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeThongDatLich.Models
{
    [Table("NgonNgus")]
    public class NGONNGU
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaNgonNgu { get; set; }

        [Required]
        [StringLength(50)]
        public string TenNgonNgu { get; set; }

        public virtual ICollection<HOSOHDV> HDVs { get; set; } = new List<HOSOHDV>();
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeThongDatLich.Models
{
    [Table("HoSoHDVs")]
    public class HOSOHDV
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaHDV { get; set; }

        [StringLength(1000)]
        public string ?GioiThieu { get; set; }

        public int ?KinhNghiem { get; set; }

        [Column(TypeName = "decimal(12, 2)")]
        public decimal GiaThue { get; set; }

        public int ?TrangThaiHoatDong { get; set; }

        public int MaPhuongXa { get; set; }
        public int MaNgonNgu { get; set; }

        [ForeignKey(nameof(MaPhuongXa))]
        public virtual PHUONGXA PhuongXa { get; set; }

        [ForeignKey(nameof(MaNgonNgu))]
        public virtual NGONNGU NgonNgu { get; set; }

        public virtual NGUOIDUNG NguoiDung { get; set; }
        public virtual ICollection<GOITOUR> GoiTours { get; set; } = new List<GOITOUR>();
        public virtual ICollection<DONDATLICH> DonDatLichs { get; set; } = new List<DONDATLICH>();
    }
}

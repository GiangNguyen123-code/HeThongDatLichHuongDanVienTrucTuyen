using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeThongDatLich.Models
{
    [Table("NguoiDungs")]
    public class NGUOIDUNG
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MaNguoiDung { get; set; }

        [Required]
        [StringLength(100)]
        public string HoTen { get; set; }

        [Required]
        [StringLength(150)]
        public string Email { get; set; }

        [Required]
        [StringLength(256)]
        public string MatKhau { get; set; }

        [StringLength(15)]
        public string SoDienThoai { get; set; }

        public string AnhDaiDien { get; set; }

        public bool TrangThaiKhoa { get; set; }

        // Mối quan hệ 1-1 mở rộng từ HDV sang NguoiDung trong SQL của bạn
        [ForeignKey(nameof(MaNguoiDung))]
        public virtual HOSOHDV HDV { get; set; }

        public virtual ICollection<PHANQUYEN> PhanQuyens { get; set; } = new List<PHANQUYEN>();
        public virtual ICollection<DONDATLICH> DonDatLichs { get; set; } = new List<DONDATLICH>();
    }
}

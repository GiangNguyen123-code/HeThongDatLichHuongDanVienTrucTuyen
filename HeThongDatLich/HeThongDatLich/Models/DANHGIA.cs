using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeThongDatLich.Models
{
    [Table("DanhGias")]
    public class DANHGIA
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MaDanhGia { get; set; }

        public int MaDatLich { get; set; }
        public int SoSao { get; set; }
        public string NoiDung { get; set; }

        [ForeignKey(nameof(MaDatLich))]
        public virtual DONDATLICH DonDatLich { get; set; }
    }
}

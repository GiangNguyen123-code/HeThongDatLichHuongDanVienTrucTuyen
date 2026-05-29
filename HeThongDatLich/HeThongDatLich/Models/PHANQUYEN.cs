using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeThongDatLich.Models
{
    [Table("PhanQuyens")]
    public class PHANQUYEN
    {
        // Khóa phức hợp sẽ được cấu hình chi tiết ở Fluent API (DbContext)
        public int MaNguoiDung { get; set; }

        public int MaQuyen { get; set; }

        [ForeignKey(nameof(MaNguoiDung))]
        public virtual NGUOIDUNG NguoiDung { get; set; }

        [ForeignKey(nameof(MaQuyen))]
        public virtual QUYEN Quyen { get; set; }
    }
}

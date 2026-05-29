using System;
using System.Collections.Generic;

namespace HeThongDatLich.Models
{
    public class ThongKeAdminViewModel
    {
        // 4 Thẻ số liệu ở trên cùng
        public int TongNguoiDung { get; set; }
        public int TongHDV { get; set; }
        public int TongBooking { get; set; }
        public decimal TongDoanhThu { get; set; }

        // Danh sách doanh thu theo từng tháng (phục vụ vẽ biểu đồ cột)
        public List<decimal> DoanhThuCacThang { get; set; }

        // Số hồ sơ hướng dẫn viên đang chờ duyệt (góc dưới bên phải)
        public int SoHosoChoDuyetHDV { get; set; }
        public int SoTaiKhoanKhoa { get; set; }
        public int SoTaiKhoanChuaCoQuyen { get; set; }
    }
}
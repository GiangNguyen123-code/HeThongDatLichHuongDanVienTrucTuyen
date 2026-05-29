using System;

namespace HeThongDatLich.Extensions
{
    public static class CurrencyExtensions
    {
        // Dành cho kiểu decimal (kiểu dữ liệu thường dùng nhất cho tiền tệ)
        public static string ToVnd(this decimal amount)
        {
            // Định dạng "N0" sẽ tự động thêm dấu phân cách hàng nghìn (VD: 1,000,000 VNĐ)
            return $"{amount:N0} VNĐ";
        }

        // Dành cho kiểu nullable decimal (decimal?) để tránh lỗi khi dữ liệu null
        public static string ToVnd(this decimal? amount)
        {
            if (!amount.HasValue)
            {
                return "0 VNĐ";
            }
            return $"{amount.Value:N0} VNĐ";
        }

        // Tùy chọn: Thêm cho kiểu double nếu bạn có sử dụng
        public static string ToVnd(this double amount)
        {
            return $"{amount:N0} VNĐ";
        }
    }
}
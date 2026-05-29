using System;

namespace HeThongDatLich.Validations
{
    public static class DatLichValidation
    {
        // Hàm kiểm tra ngày đặt lịch không được phép nằm trong quá khứ
        public static bool IsValidNgayDat(DateTime ngayDat, out string errorMessage)
        {
            // So sánh phần Ngày (Date) để người dùng vẫn có thể đặt được lịch trong ngày hôm nay
            if (ngayDat.Date < DateTime.Now.Date)
            {
                errorMessage = "Ngày đặt lịch không hợp lệ. Vui lòng chọn ngày từ hôm nay trở đi!";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }
    }
}
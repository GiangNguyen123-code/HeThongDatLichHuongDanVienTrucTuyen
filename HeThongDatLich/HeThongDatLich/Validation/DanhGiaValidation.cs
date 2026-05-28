using System.Text.RegularExpressions;

namespace HeThongDatLich.Validations
{
    public static class DanhGiaValidation
    {
        public static bool IsValidNoiDung(string noiDung, out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(noiDung))
            {
                errorMessage = "Nội dung đánh giá không được để trống.";
                return false;
            }

            // Regex kiểm tra ký tự đầu tiên: \p{L} đại diện cho bất kỳ chữ cái nào (bao gồm cả tiếng Việt có dấu)
            if (!Regex.IsMatch(noiDung.Trim(), @"^\p{L}"))
            {
                errorMessage = "Nội dung đánh giá phải bắt đầu bằng một chữ cái (không dùng số hoặc ký tự đặc biệt ở đầu).";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }
    }
}
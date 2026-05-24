# 🗺️ Đề tài: Xây dựng hệ thống đặt lịch Hướng dẫn viên trực tuyến

**Nhóm:** 10  
**Thành viên thực hiện:**
* **Nguyễn Đỗ Hữu Giang** 
* **Lê Doãn Đàn**
* **Thái Trường Giang**

---

## 📝 1. Giới thiệu tổng quan
> Hệ thống Đặt lịch Hướng dẫn viên trực tuyến là một nền tảng web giúp kết nối trực tiếp du khách và hướng dẫn viên chuyên nghiệp một cách nhanh chóng, minh bạch và tiện lợi.

**Các tính năng cốt lõi:**
* **Tìm kiếm thông minh:** Lọc theo địa điểm, ngôn ngữ và chuyên môn.
* **Hồ sơ minh bạch:** Xem chi tiết Profile và các đánh giá từ khách hàng cũ.
* **Đặt lịch linh hoạt:** Lựa chọn theo khung giờ trống của hướng dẫn viên.
* **Quản trị toàn diện:** Thống kê doanh thu và quản lý đơn đặt cho Admin.

## 🚩 2. Xác định vấn đề thực tế
Dự án tập trung giải quyết các "điểm nghẽn" sau trong ngành du lịch:
* **Thiếu minh bạch:** Du khách mất thời gian tìm kiếm qua trung gian với giá cả không rõ ràng.
* **Khó quản lý:** Hướng dẫn viên thiếu công cụ chuyên dụng để theo dõi lịch trình và thu nhập.
* **Số hóa quy trình:** Chuyển đổi cách kết nối truyền thống sang nền tảng số để quản lý trạng thái đơn hàng và doanh thu hiệu quả hơn.

## 👥 3. Đối tượng sử dụng chính
Hệ thống phân quyền rõ ràng cho 3 nhóm đối tượng:

1. **Khách du lịch (Khách hàng):** * Sử dụng bộ lọc thông minh (giá, ngôn ngữ, địa điểm).
    * Thanh toán an toàn qua các cổng trực tuyến.
2. **Hướng dẫn viên du lịch:** * Xây dựng hồ sơ năng lực (Portfolio).
    * Quản lý lịch trống và nhận yêu cầu trực tiếp từ khách.
3. **Quản trị viên (Admin):** * Kiểm duyệt hồ sơ đối tác và quản lý giao dịch.
    * Giải quyết khiếu nại và vận hành hệ thống.

## 💻 4. Công nghệ sử dụng
Dự án được xây dựng trên nền tảng công nghệ hiện đại:
* **Backend:** ASP.NET Core MVC (C# / .NET 8).
* **Database:** Microsoft SQL Server 2019.
* **Frontend:** HTML5, CSS3, JavaScript (Bootstrap 5).
* **Tương tác CSDL:** Entity Framework Core.
* **Tích hợp:** Google Maps API & GPS Integration cho bản đồ và định vị.

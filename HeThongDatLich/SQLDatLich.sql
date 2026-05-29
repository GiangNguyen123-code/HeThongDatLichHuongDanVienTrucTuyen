USE [HeThongDatLichDB]
GO

-- ==============================================================================
-- BƯỚC 1: TẠM THỜI TẮT KIỂM TRA KHÓA NGOẠI
-- ==============================================================================
EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL';
GO

-- ==============================================================================
-- BƯỚC 2: XOÁ TOÀN BỘ DỮ LIỆU CŨ 
-- ==============================================================================
DELETE FROM DanhGias;
DELETE FROM HoaDons;
DELETE FROM DonDatLichs;
DELETE FROM MaGiamGias;
DELETE FROM GoiTours;
DELETE FROM HoSoHDVs;
DELETE FROM PhanQuyens;
DELETE FROM NguoiDungs;
DELETE FROM Quyens;
DELETE FROM PhuongThucThanhToans;
DELETE FROM NgonNgus;
DELETE FROM PhuongXas;
DELETE FROM TinhThanhs;
GO

-- ==============================================================================
-- BƯỚC 3: RESET BỘ ĐẾM ID THÔNG MINH (TRỊ DỨT ĐIỂM BỆNH PHẢI CHẠY 2 LẦN)
-- ==============================================================================
DECLARE @TableName NVARCHAR(128);
DECLARE cur CURSOR FOR 
    SELECT SCHEMA_NAME(schema_id) + '.' + name 
    FROM sys.tables 
    WHERE OBJECTPROPERTY(object_id, 'TableHasIdentity') = 1;

OPEN cur;
FETCH NEXT FROM cur INTO @TableName;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- Nếu bảng mới tinh (chưa từng có dữ liệu) -> Ép ID mốc là 1
    IF EXISTS (SELECT 1 FROM sys.identity_columns WHERE object_id = OBJECT_ID(@TableName) AND last_value IS NULL)
        DBCC CHECKIDENT (@TableName, RESEED, 1);
    -- Nếu bảng đã từng có dữ liệu và vừa bị DELETE -> Ép ID mốc là 0 (để 0 + 1 = 1)
    ELSE
        DBCC CHECKIDENT (@TableName, RESEED, 0);
        
    FETCH NEXT FROM cur INTO @TableName;
END;

CLOSE cur;
DEALLOCATE cur;
GO

-- ==============================================================================
-- BƯỚC 4: BẬT LẠI KIỂM TRA KHÓA NGOẠI ĐỂ BẢO VỆ DỮ LIỆU
-- ==============================================================================
EXEC sp_MSforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL';
GO

-- ==============================================================================
-- BƯỚC 5: THÊM DỮ LIỆU DANH MỤC CƠ BẢN
-- ==============================================================================
INSERT INTO Quyens (TenQuyen) VALUES ('Admin'), ('KhachHang'), ('HDV');

INSERT INTO NgonNgus (TenNgonNgu) VALUES (N'Tiếng Việt'), (N'Tiếng Anh'), (N'Tiếng Nhật'), (N'Tiếng Hàn'), (N'Tiếng Trung'), (N'Tiếng Pháp');

INSERT INTO PhuongThucThanhToans (TenPhuongThuc, GhiChu) VALUES 
(N'Tiền mặt', N'Thanh toán trực tiếp cho HDV'), 
(N'Chuyển khoản VNPay', N'Chuyển khoản qua cổng thanh toán VNPay'),
(N'Ví điện tử MoMo', N'Quét mã QR qua ứng dụng MoMo');

INSERT INTO TinhThanhs (TenTinh) VALUES 
(N'Hà Nội'), (N'Đà Nẵng'), (N'TP. Hồ Chí Minh'), (N'Hội An'), (N'Huế'), (N'Đà Lạt'), (N'Nha Trang'), (N'Sa Pa');

INSERT INTO PhuongXas (TenPhuongXa, MaTinh) VALUES 
(N'Quận Hoàn Kiếm', 1), (N'Quận Ba Đình', 1),
(N'Quận Hải Châu', 2), (N'Quận Sơn Trà', 2),
(N'Quận 1', 3), (N'Thành phố Thủ Đức', 3),
(N'Phường Minh An', 4), (N'Phường Cẩm Phô', 4),
(N'Phường Vĩ Dạ', 5),
(N'Phường 1 (Chợ Đà Lạt)', 6),
(N'Phường Lộc Thọ', 7),
(N'Phường Sa Pa', 8);

-- ==============================================================================
-- BƯỚC 6: THÊM TÀI KHOẢN NGƯỜI DÙNG 
-- ==============================================================================
INSERT INTO NguoiDungs (HoTen, Email, MatKhau, SoDienThoai, AnhDaiDien, TrangThaiKhoa) VALUES 
(N'Admin Portal', 'admin@gmail.com', '123456', '0787588867', 'Avatar_Text', 0),         -- ID 1 (Admin)
(N'Nguyễn Văn A', 'nguyenvana@gmail.com', '123456', '0905556666', 'Avatar_Text', 0),    -- ID 2 (Khách hàng)
(N'Kẻ Phá Hoại', 'baduser@gmail.com', '123456', '0999999999', 'Avatar_Text', 1),        -- ID 3 (Bị khóa)
(N'Chưa Rõ Quyền', 'no-role@gmail.com', '123456', '0888888888', 'Avatar_Text', 0),      -- ID 4 (Chưa phân quyền)
(N'Trường Giang', 'truonggiang@gmail.com', '123456', '0988111222', 'Avatar_Text', 0),   -- ID 5 (Khách hàng thật)
(N'Trần Bình', 'tranbinh@gmail.com', '123456', '0901112222', 'Avatar_Text', 0),         -- ID 6 (HDV)
(N'Lê Hoa', 'lehoa@gmail.com', '123456', '0903334444', 'Avatar_Text', 0),               -- ID 7 (HDV)
(N'Nguyễn Thu Hà', 'thuha.hanoi@gmail.com', '123456', '0911223344', 'https://images.unsplash.com/photo-1544005313-94ddf0286df2?w=400', 0), -- ID 8
(N'Lê Bảo Anh', 'baoanh.dn@gmail.com', '123456', '0922334455', 'https://images.unsplash.com/photo-1580894742597-87bc8789db3d?w=400', 0),   -- ID 9
(N'Trần Minh Quân', 'minhquan.sg@gmail.com', '123456', '0933445566', 'https://images.unsplash.com/photo-1500648767791-00dcc994a43e?w=400', 0), -- ID 10
(N'Phạm Thuỳ Dương', 'thuyduong.ha@gmail.com', '123456', '0944556677', 'https://images.unsplash.com/photo-1534528741775-53994a69daeb?w=400', 0), -- ID 11
(N'Vũ Công Thành', 'thanhvu.dl@gmail.com', '123456', '0955667788', 'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=400', 0),   -- ID 12
(N'A Páo', 'apao.sapa@gmail.com', '123456', '0966778899', 'https://images.unsplash.com/photo-1506794778202-cad84cf45f1d?w=400', 0);          -- ID 13

-- ==============================================================================
-- BƯỚC 7: PHÂN QUYỀN & HỒ SƠ 
-- ==============================================================================
INSERT INTO PhanQuyens (MaNguoiDung, MaQuyen) VALUES 
(1, 1), 
(2, 2), (3, 2), (5, 2), 
(6, 3), (7, 3), (8, 3), (9, 3), (10, 3), (11, 3), (12, 3), (13, 3); 

INSERT INTO HoSoHDVs (MaHDV, GioiThieu, KinhNghiem, GiaThue, TrangThaiHoatDong, MaPhuongXa, MaNgonNgu) VALUES 
(6, N'Xin chào, tôi là Bình, thích phượt.', 2, 500000, 0, 1, 1), 
(7, N'Xin chào, tôi là Hoa, chuyên tour Đà Nẵng.',2, 800000, 1, 3, 2),
(8, N'Tôi sẽ đưa bạn khám phá 36 phố phường Hà Nội qua những câu chuyện lịch sử thú vị nhất.', 5, 450000, 1, 1, 2),
(9, N'Đam mê nhiếp ảnh và ẩm thực, kiêm luôn "thợ chụp ảnh" siêu có tâm.', 3, 500000, 1, 3, 1),
(10, N'Sống tại Sài Gòn hơn 10 năm, tôi biết rõ những con hẻm nghệ thuật không phải ai cũng biết.', 6, 600000, 1, 5, 4),
(11, N'Hội An trong mắt tôi bình yên. Cùng tôi thưởng thức Mì Quảng đúng điệu nhé.', 4, 400000, 1, 7, 3),
(12, N'Thổ địa Đà Lạt chính hiệu. Chuyên dẫn các tour săn mây, cắm trại.', 2, 350000, 1, 10, 1),
(13, N'Sẵn sàng đưa bạn trekking qua những bản làng đẹp nhất Sa Pa.', 8, 700000, 1, 12, 2);

-- ==============================================================================
-- BƯỚC 8: TOUR, VOUCHER & ĐƠN ĐẶT LỊCH
-- ==============================================================================
INSERT INTO GoiTours (MaHDV, TenTour, MoTa, GiaTien, ThoiGian) VALUES 
(8, N'Hanoi Old Quarter Walking Tour', N'Đi bộ ngắm phố cổ, ăn Phở, uống Cà phê', 350000, N'4 Tiếng'), 
(9, N'Đà Nẵng City Tour & Food Tour', N'Chợ Cồn, Cầu Rồng, Sơn Trà', 400000, N'Nửa ngày'),         
(12, N'Tour Săn Mây Cầu Đất', N'Đón bình minh, uống cà phê mây', 300000, N'Sáng (4h - 9h)');       

INSERT INTO MaGiamGias (TenVoucher, PhanTramGiam, GiamToiDa, NgayHetHan, SoLuong) VALUES 
('SUMMER2026', 10, 50000, '2026-12-31', 100),
('VIPGIANG', 50, 200000, '2026-12-31', 50);

INSERT INTO DonDatLichs (MaKhachHang, MaHDV, MaVoucher, MaTour, NgayDat, KhungGio, GhiChu, TrangThai) VALUES 
(2, 7, NULL, NULL, '2026-04-01', N'Cả ngày', N'Đơn test biểu đồ T4', 3), 
(2, 7, NULL, NULL, '2026-05-01', N'Cả ngày', N'Đơn test biểu đồ T5', 3), 
(2, 7, NULL, NULL, '2026-05-15', N'Cả ngày', N'Đơn test biểu đồ T5', 3), 
(2, 7, NULL, NULL, '2026-06-01', N'Cả ngày', N'Đơn test biểu đồ T6', 3), 
(2, 7, NULL, NULL, '2026-07-01', N'Cả ngày', N'Đơn test biểu đồ T7', 3), 
(5, 9, NULL, 2, '2026-05-10', N'Cả ngày (08:00 - 17:00)', N'Chuẩn bị xe nha', 3),
(5, 12, NULL, 3, '2026-12-20', N'Sáng (04:00 - 09:00)', N'Săn mây sớm', 1), 
(5, 8, 2, 1, '2026-01-15', N'Chiều (13:00 - 18:00)', N'Dùng Voucher', 3), 
(5, 13, NULL, NULL, '2026-06-01', N'Cả ngày', N'Bận đột xuất', 4); 

INSERT INTO HoaDons (MaDatLich, TongTien, NgayThanhToan, TrangThaiTT, SoTienGiam, MaPTTT) VALUES 
(1, 1500000, '2026-04-15', 1, 0, 1), 
(2, 2800000, '2026-05-10', 1, 0, 1), 
(3, 1200000, '2026-05-25', 1, 0, 1), 
(4, 5500000, '2026-06-12', 1, 0, 1), 
(5, 3200000, '2026-07-08', 1, 0, 1),
(6, 400000, '2026-05-10', 1, 0, 1),
(7, 300000, NULL, 0, 0, 2),
(8, 250000, '2026-01-15', 1, 200000, 3),
(9, 700000, NULL, 0, 0, 1);

INSERT INTO DanhGias (MaDatLich, SoSao, NoiDung) VALUES 
(8, 5, N'Chuyến đi quá tuyệt vời! Cảm ơn bạn HDV rất nhiều.'),
(6, 4, N'Tour ổn, đồ ăn ngon nhưng thời tiết hơi nóng.');
GO
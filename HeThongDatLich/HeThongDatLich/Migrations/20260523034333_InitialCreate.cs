using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongDatLich.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MaGiamGias",
                columns: table => new
                {
                    MaVoucher = table.Column<int>(type: "int", nullable: false),
                    TenVoucher = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhanTramGiam = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GiamToiDa = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NgayHetHan = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaGiamGias", x => x.MaVoucher);
                });

            migrationBuilder.CreateTable(
                name: "NgonNgus",
                columns: table => new
                {
                    MaNgonNgu = table.Column<int>(type: "int", nullable: false),
                    TenNgonNgu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NgonNgus", x => x.MaNgonNgu);
                });

            migrationBuilder.CreateTable(
                name: "PhuongThucThanhToans",
                columns: table => new
                {
                    MaPTTT = table.Column<int>(type: "int", nullable: false),
                    TenPhuongThuc = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhuongThucThanhToans", x => x.MaPTTT);
                });

            migrationBuilder.CreateTable(
                name: "Quyens",
                columns: table => new
                {
                    MaQuyen = table.Column<int>(type: "int", nullable: false),
                    TenQuyen = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quyens", x => x.MaQuyen);
                });

            migrationBuilder.CreateTable(
                name: "TinhThanhs",
                columns: table => new
                {
                    MaTinh = table.Column<int>(type: "int", nullable: false),
                    TenTinh = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TinhThanhs", x => x.MaTinh);
                });

            migrationBuilder.CreateTable(
                name: "PhuongXas",
                columns: table => new
                {
                    MaPhuongXa = table.Column<int>(type: "int", nullable: false),
                    TenPhuongXa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MaTinh = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhuongXas", x => x.MaPhuongXa);
                    table.ForeignKey(
                        name: "FK_PhuongXas_TinhThanhs_MaTinh",
                        column: x => x.MaTinh,
                        principalTable: "TinhThanhs",
                        principalColumn: "MaTinh",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HoSoHDVs",
                columns: table => new
                {
                    MaHDV = table.Column<int>(type: "int", nullable: false),
                    GioiThieu = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    KinhNghiem = table.Column<int>(type: "int", nullable: false),
                    GiaThue = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    TrangThaiHoatDong = table.Column<int>(type: "int", nullable: false),
                    MaPhuongXa = table.Column<int>(type: "int", nullable: false),
                    MaNgonNgu = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoHDVs", x => x.MaHDV);
                    table.ForeignKey(
                        name: "FK_HoSoHDVs_NgonNgus_MaNgonNgu",
                        column: x => x.MaNgonNgu,
                        principalTable: "NgonNgus",
                        principalColumn: "MaNgonNgu",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HoSoHDVs_PhuongXas_MaPhuongXa",
                        column: x => x.MaPhuongXa,
                        principalTable: "PhuongXas",
                        principalColumn: "MaPhuongXa",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GoiTours",
                columns: table => new
                {
                    MaTour = table.Column<int>(type: "int", nullable: false),
                    MaHDV = table.Column<int>(type: "int", nullable: false),
                    TenTour = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    GiaTien = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    ThoiGian = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoiTours", x => x.MaTour);
                    table.ForeignKey(
                        name: "FK_GoiTours_HoSoHDVs_MaHDV",
                        column: x => x.MaHDV,
                        principalTable: "HoSoHDVs",
                        principalColumn: "MaHDV",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NguoiDungs",
                columns: table => new
                {
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    AnhDaiDien = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThaiKhoa = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguoiDungs", x => x.MaNguoiDung);
                    table.ForeignKey(
                        name: "FK_NguoiDungs_HoSoHDVs_MaNguoiDung",
                        column: x => x.MaNguoiDung,
                        principalTable: "HoSoHDVs",
                        principalColumn: "MaHDV",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DonDatLichs",
                columns: table => new
                {
                    MaDatLich = table.Column<int>(type: "int", nullable: false),
                    MaKhachHang = table.Column<int>(type: "int", nullable: false),
                    MaHDV = table.Column<int>(type: "int", nullable: false),
                    MaVoucher = table.Column<int>(type: "int", nullable: true),
                    MaTour = table.Column<int>(type: "int", nullable: true),
                    NgayDat = table.Column<DateTime>(type: "date", nullable: false),
                    KhungGio = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TrangThai = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonDatLichs", x => x.MaDatLich);
                    table.ForeignKey(
                        name: "FK_DonDatLichs_GoiTours_MaTour",
                        column: x => x.MaTour,
                        principalTable: "GoiTours",
                        principalColumn: "MaTour");
                    table.ForeignKey(
                        name: "FK_DonDatLichs_HoSoHDVs_MaHDV",
                        column: x => x.MaHDV,
                        principalTable: "HoSoHDVs",
                        principalColumn: "MaHDV",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DonDatLichs_MaGiamGias_MaVoucher",
                        column: x => x.MaVoucher,
                        principalTable: "MaGiamGias",
                        principalColumn: "MaVoucher");
                    table.ForeignKey(
                        name: "FK_DonDatLichs_NguoiDungs_MaKhachHang",
                        column: x => x.MaKhachHang,
                        principalTable: "NguoiDungs",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PhanQuyens",
                columns: table => new
                {
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    MaQuyen = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhanQuyens", x => new { x.MaNguoiDung, x.MaQuyen });
                    table.ForeignKey(
                        name: "FK_PhanQuyens_NguoiDungs_MaNguoiDung",
                        column: x => x.MaNguoiDung,
                        principalTable: "NguoiDungs",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhanQuyens_Quyens_MaQuyen",
                        column: x => x.MaQuyen,
                        principalTable: "Quyens",
                        principalColumn: "MaQuyen",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DanhGias",
                columns: table => new
                {
                    MaDanhGia = table.Column<int>(type: "int", nullable: false),
                    MaDatLich = table.Column<int>(type: "int", nullable: false),
                    SoSao = table.Column<int>(type: "int", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhGias", x => x.MaDanhGia);
                    table.ForeignKey(
                        name: "FK_DanhGias_DonDatLichs_MaDatLich",
                        column: x => x.MaDatLich,
                        principalTable: "DonDatLichs",
                        principalColumn: "MaDatLich",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HoaDons",
                columns: table => new
                {
                    MaHoaDon = table.Column<int>(type: "int", nullable: false),
                    MaDatLich = table.Column<int>(type: "int", nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    NgayThanhToan = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    TrangThaiTT = table.Column<bool>(type: "bit", nullable: false),
                    SoTienGiam = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaPTTT = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoaDons", x => x.MaHoaDon);
                    table.ForeignKey(
                        name: "FK_HoaDons_DonDatLichs_MaDatLich",
                        column: x => x.MaDatLich,
                        principalTable: "DonDatLichs",
                        principalColumn: "MaDatLich",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HoaDons_PhuongThucThanhToans_MaPTTT",
                        column: x => x.MaPTTT,
                        principalTable: "PhuongThucThanhToans",
                        principalColumn: "MaPTTT",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DanhGias_MaDatLich",
                table: "DanhGias",
                column: "MaDatLich");

            migrationBuilder.CreateIndex(
                name: "IX_DonDatLichs_MaHDV",
                table: "DonDatLichs",
                column: "MaHDV");

            migrationBuilder.CreateIndex(
                name: "IX_DonDatLichs_MaKhachHang",
                table: "DonDatLichs",
                column: "MaKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_DonDatLichs_MaTour",
                table: "DonDatLichs",
                column: "MaTour");

            migrationBuilder.CreateIndex(
                name: "IX_DonDatLichs_MaVoucher",
                table: "DonDatLichs",
                column: "MaVoucher");

            migrationBuilder.CreateIndex(
                name: "IX_GoiTours_MaHDV",
                table: "GoiTours",
                column: "MaHDV");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_MaDatLich",
                table: "HoaDons",
                column: "MaDatLich",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_MaPTTT",
                table: "HoaDons",
                column: "MaPTTT");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoHDVs_MaNgonNgu",
                table: "HoSoHDVs",
                column: "MaNgonNgu");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoHDVs_MaPhuongXa",
                table: "HoSoHDVs",
                column: "MaPhuongXa");

            migrationBuilder.CreateIndex(
                name: "IX_PhanQuyens_MaQuyen",
                table: "PhanQuyens",
                column: "MaQuyen");

            migrationBuilder.CreateIndex(
                name: "IX_PhuongXas_MaTinh",
                table: "PhuongXas",
                column: "MaTinh");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DanhGias");

            migrationBuilder.DropTable(
                name: "HoaDons");

            migrationBuilder.DropTable(
                name: "PhanQuyens");

            migrationBuilder.DropTable(
                name: "DonDatLichs");

            migrationBuilder.DropTable(
                name: "PhuongThucThanhToans");

            migrationBuilder.DropTable(
                name: "Quyens");

            migrationBuilder.DropTable(
                name: "GoiTours");

            migrationBuilder.DropTable(
                name: "MaGiamGias");

            migrationBuilder.DropTable(
                name: "NguoiDungs");

            migrationBuilder.DropTable(
                name: "HoSoHDVs");

            migrationBuilder.DropTable(
                name: "NgonNgus");

            migrationBuilder.DropTable(
                name: "PhuongXas");

            migrationBuilder.DropTable(
                name: "TinhThanhs");
        }
    }
}

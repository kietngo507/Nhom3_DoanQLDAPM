using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITShop.Migrations
{
    /// <inheritdoc />
    public partial class TrangTin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChuDe",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenChuDe = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TenChuDeKhongDau = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChuDe", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "BaiViet",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChuDeID = table.Column<int>(type: "int", nullable: false),
                    NguoiDungID = table.Column<int>(type: "int", nullable: false),
                    TieuDe = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TieuDeKhongDau = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TomTat = table.Column<string>(type: "ntext", nullable: false),
                    NoiDung = table.Column<string>(type: "ntext", nullable: false),
                    NgayDang = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LuotXem = table.Column<int>(type: "int", nullable: false),
                    KiemDuyet = table.Column<bool>(type: "bit", nullable: false),
                    HienThi = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaiViet", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BaiViet_ChuDe_ChuDeID",
                        column: x => x.ChuDeID,
                        principalTable: "ChuDe",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BaiViet_NguoiDung_NguoiDungID",
                        column: x => x.NguoiDungID,
                        principalTable: "NguoiDung",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BinhLuanBaiViet",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BaiVietID = table.Column<int>(type: "int", nullable: false),
                    NguoiDungID = table.Column<int>(type: "int", nullable: false),
                    NoiDungBinhLuan = table.Column<string>(type: "ntext", nullable: false),
                    NgayDang = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LuotXem = table.Column<int>(type: "int", nullable: false),
                    KiemDuyet = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BinhLuanBaiViet", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BinhLuanBaiViet_BaiViet_BaiVietID",
                        column: x => x.BaiVietID,
                        principalTable: "BaiViet",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BinhLuanBaiViet_NguoiDung_NguoiDungID",
                        column: x => x.NguoiDungID,
                        principalTable: "NguoiDung",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BaiViet_ChuDeID",
                table: "BaiViet",
                column: "ChuDeID");

            migrationBuilder.CreateIndex(
                name: "IX_BaiViet_NguoiDungID",
                table: "BaiViet",
                column: "NguoiDungID");

            migrationBuilder.CreateIndex(
                name: "IX_BinhLuanBaiViet_BaiVietID",
                table: "BinhLuanBaiViet",
                column: "BaiVietID");

            migrationBuilder.CreateIndex(
                name: "IX_BinhLuanBaiViet_NguoiDungID",
                table: "BinhLuanBaiViet",
                column: "NguoiDungID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BinhLuanBaiViet");

            migrationBuilder.DropTable(
                name: "BaiViet");

            migrationBuilder.DropTable(
                name: "ChuDe");
        }
    }
}

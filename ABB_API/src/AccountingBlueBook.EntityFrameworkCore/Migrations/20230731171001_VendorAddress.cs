using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountingBlueBook.Migrations
{
    /// <inheritdoc />
    public partial class VendorAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VendorAddresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompleteAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(240)", maxLength: 240, nullable: true),
                    State = table.Column<string>(type: "nvarchar(240)", maxLength: 240, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(240)", maxLength: 240, nullable: true),
                    PostCode = table.Column<string>(type: "nvarchar(240)", maxLength: 240, nullable: true),
                    Fax = table.Column<string>(type: "nvarchar(240)", maxLength: 240, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(240)", maxLength: 240, nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    VendorId = table.Column<int>(type: "int", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorAddresses_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_VendorAddresses_VendorId",
                table: "VendorAddresses",
                column: "VendorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VendorAddresses");
        }
    }
}

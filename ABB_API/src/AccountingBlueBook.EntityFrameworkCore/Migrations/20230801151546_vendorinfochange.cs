using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountingBlueBook.Migrations
{
    /// <inheritdoc />
    public partial class vendorinfochange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VendorId",
                table: "VendorContactInfos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VendorContactInfos_VendorId",
                table: "VendorContactInfos",
                column: "VendorId");

            migrationBuilder.AddForeignKey(
                name: "FK_VendorContactInfos_Vendors_VendorId",
                table: "VendorContactInfos",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VendorContactInfos_Vendors_VendorId",
                table: "VendorContactInfos");

            migrationBuilder.DropIndex(
                name: "IX_VendorContactInfos_VendorId",
                table: "VendorContactInfos");

            migrationBuilder.DropColumn(
                name: "VendorId",
                table: "VendorContactInfos");
        }
    }
}

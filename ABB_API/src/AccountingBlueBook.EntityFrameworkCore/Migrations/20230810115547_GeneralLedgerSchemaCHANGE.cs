using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountingBlueBook.Migrations
{
    /// <inheritdoc />
    public partial class GeneralLedgerSchemaCHANGE : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VoucherId",
                table: "GeneralLedgers",
                newName: "CreatedBy");

            migrationBuilder.AddColumn<string>(
                name: "CreatorUserName",
                table: "GeneralLedgers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VoucherNo",
                table: "GeneralLedgers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatorUserName",
                table: "GeneralLedgers");

            migrationBuilder.DropColumn(
                name: "VoucherNo",
                table: "GeneralLedgers");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "GeneralLedgers",
                newName: "VoucherId");
        }
    }
}

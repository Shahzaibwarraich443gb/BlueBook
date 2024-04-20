using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountingBlueBook.Migrations
{
    /// <inheritdoc />
    public partial class CheckDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CheckAccountDetail_Checks_CheckId",
                table: "CheckAccountDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_CheckProductDetail_Checks_CheckId",
                table: "CheckProductDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CheckProductDetail",
                table: "CheckProductDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CheckAccountDetail",
                table: "CheckAccountDetail");

            migrationBuilder.RenameTable(
                name: "CheckProductDetail",
                newName: "CheckProductDetails");

            migrationBuilder.RenameTable(
                name: "CheckAccountDetail",
                newName: "CheckAccountDetails");

            migrationBuilder.RenameIndex(
                name: "IX_CheckProductDetail_CheckId",
                table: "CheckProductDetails",
                newName: "IX_CheckProductDetails_CheckId");

            migrationBuilder.RenameIndex(
                name: "IX_CheckAccountDetail_CheckId",
                table: "CheckAccountDetails",
                newName: "IX_CheckAccountDetails_CheckId");

            migrationBuilder.AlterColumn<long>(
                name: "CheckId",
                table: "CheckProductDetails",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "CheckId",
                table: "CheckAccountDetails",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CheckProductDetails",
                table: "CheckProductDetails",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CheckAccountDetails",
                table: "CheckAccountDetails",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CheckAccountDetails_Checks_CheckId",
                table: "CheckAccountDetails",
                column: "CheckId",
                principalTable: "Checks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CheckProductDetails_Checks_CheckId",
                table: "CheckProductDetails",
                column: "CheckId",
                principalTable: "Checks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CheckAccountDetails_Checks_CheckId",
                table: "CheckAccountDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_CheckProductDetails_Checks_CheckId",
                table: "CheckProductDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CheckProductDetails",
                table: "CheckProductDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CheckAccountDetails",
                table: "CheckAccountDetails");

            migrationBuilder.RenameTable(
                name: "CheckProductDetails",
                newName: "CheckProductDetail");

            migrationBuilder.RenameTable(
                name: "CheckAccountDetails",
                newName: "CheckAccountDetail");

            migrationBuilder.RenameIndex(
                name: "IX_CheckProductDetails_CheckId",
                table: "CheckProductDetail",
                newName: "IX_CheckProductDetail_CheckId");

            migrationBuilder.RenameIndex(
                name: "IX_CheckAccountDetails_CheckId",
                table: "CheckAccountDetail",
                newName: "IX_CheckAccountDetail_CheckId");

            migrationBuilder.AlterColumn<long>(
                name: "CheckId",
                table: "CheckProductDetail",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "CheckId",
                table: "CheckAccountDetail",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CheckProductDetail",
                table: "CheckProductDetail",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CheckAccountDetail",
                table: "CheckAccountDetail",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CheckAccountDetail_Checks_CheckId",
                table: "CheckAccountDetail",
                column: "CheckId",
                principalTable: "Checks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CheckProductDetail_Checks_CheckId",
                table: "CheckProductDetail",
                column: "CheckId",
                principalTable: "Checks",
                principalColumn: "Id");
        }
    }
}

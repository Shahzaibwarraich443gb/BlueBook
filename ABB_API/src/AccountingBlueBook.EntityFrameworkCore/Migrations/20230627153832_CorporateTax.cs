using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountingBlueBook.Migrations
{
    /// <inheritdoc />
    public partial class CorporateTax : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IncomeDetails_PersonalTax_PersonalTaxId",
                table: "IncomeDetails");

            migrationBuilder.AlterColumn<long>(
                name: "PersonalTaxId",
                table: "IncomeDetails",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "CorporateTax",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    FinancialYear = table.Column<int>(type: "int", nullable: false),
                    MonthlyData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LegalStatus = table.Column<int>(type: "int", nullable: true),
                    Tenure = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_CorporateTax", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CorporateTax_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CorporateTax_CustomerId",
                table: "CorporateTax",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_IncomeDetails_PersonalTax_PersonalTaxId",
                table: "IncomeDetails",
                column: "PersonalTaxId",
                principalTable: "PersonalTax",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IncomeDetails_PersonalTax_PersonalTaxId",
                table: "IncomeDetails");

            migrationBuilder.DropTable(
                name: "CorporateTax");

            migrationBuilder.AlterColumn<long>(
                name: "PersonalTaxId",
                table: "IncomeDetails",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_IncomeDetails_PersonalTax_PersonalTaxId",
                table: "IncomeDetails",
                column: "PersonalTaxId",
                principalTable: "PersonalTax",
                principalColumn: "Id");
        }
    }
}

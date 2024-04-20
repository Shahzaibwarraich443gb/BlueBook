using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountingBlueBook.Migrations
{
    /// <inheritdoc />
    public partial class salesTaxSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CalendarEvents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    refModuleId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    refReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    refUserId = table.Column<long>(type: "bigint", nullable: true),
                    refCompanyId = table.Column<long>(type: "bigint", nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Start = table.Column<DateTime>(type: "datetime2", nullable: true),
                    End = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Meridian1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Meridian2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: true),
                    ThemeColor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlertBefore = table.Column<int>(type: "int", nullable: true),
                    AlertBeforeTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsAlertBefore = table.Column<bool>(type: "bit", nullable: true),
                    SnoozeTime = table.Column<int>(type: "int", nullable: true),
                    IsSnooze = table.Column<bool>(type: "bit", nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsRepeat = table.Column<bool>(type: "bit", nullable: true),
                    RepeatType = table.Column<int>(type: "int", nullable: true),
                    ISUPCOMINGEVENT = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SalesTaxes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    FinancialYear = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LegalStatus = table.Column<int>(type: "int", nullable: true),
                    TenureForm = table.Column<int>(type: "int", nullable: false),
                    TotalMonthlyAmount = table.Column<double>(type: "float", nullable: false),
                    NonTaxableAmount = table.Column<double>(type: "float", nullable: false),
                    TaxableSales = table.Column<double>(type: "float", nullable: false),
                    SalesTaxAmount = table.Column<double>(type: "float", nullable: false),
                    SalesRatePercentage = table.Column<double>(type: "float", nullable: false),
                    TaxDataMonthly = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_SalesTaxes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesTaxes_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SalesTaxes_CustomerId",
                table: "SalesTaxes",
                column: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CalendarEvents");

            migrationBuilder.DropTable(
                name: "SalesTaxes");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountingBlueBook.Migrations
{
    /// <inheritdoc />
    public partial class personalTaxSchemaIncomeDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PersonalTaxId",
                table: "Dependents",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PersonalTax",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    FinancialYear = table.Column<int>(type: "int", nullable: false),
                    TaxFillingStatus = table.Column<int>(type: "int", nullable: false),
                    Form = table.Column<int>(type: "int", nullable: false),
                    Tenure = table.Column<int>(type: "int", nullable: false),
                    FilerOccupation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoutingNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilersLicenseNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IssueState = table.Column<int>(type: "int", nullable: false),
                    ThreeDigitCode = table.Column<int>(type: "int", nullable: false),
                    OtherExpense = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpouseId = table.Column<long>(type: "bigint", nullable: false),
                    DependentIds = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IncomeDetailIds = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_PersonalTax", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonalTax_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonalTax_Spouses_SpouseId",
                        column: x => x.SpouseId,
                        principalTable: "Spouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IncomeDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    IncomeDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    FederalWH = table.Column<int>(type: "int", nullable: false),
                    StateWH = table.Column<int>(type: "int", nullable: false),
                    PersonalTaxId = table.Column<long>(type: "bigint", nullable: true),
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
                    table.PrimaryKey("PK_IncomeDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncomeDetails_PersonalTax_PersonalTaxId",
                        column: x => x.PersonalTaxId,
                        principalTable: "PersonalTax",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dependents_PersonalTaxId",
                table: "Dependents",
                column: "PersonalTaxId");

            migrationBuilder.CreateIndex(
                name: "IX_IncomeDetails_PersonalTaxId",
                table: "IncomeDetails",
                column: "PersonalTaxId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonalTax_CustomerId",
                table: "PersonalTax",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonalTax_SpouseId",
                table: "PersonalTax",
                column: "SpouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dependents_PersonalTax_PersonalTaxId",
                table: "Dependents",
                column: "PersonalTaxId",
                principalTable: "PersonalTax",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dependents_PersonalTax_PersonalTaxId",
                table: "Dependents");

            migrationBuilder.DropTable(
                name: "IncomeDetails");

            migrationBuilder.DropTable(
                name: "PersonalTax");

            migrationBuilder.DropIndex(
                name: "IX_Dependents_PersonalTaxId",
                table: "Dependents");

            migrationBuilder.DropColumn(
                name: "PersonalTaxId",
                table: "Dependents");
        }
    }
}

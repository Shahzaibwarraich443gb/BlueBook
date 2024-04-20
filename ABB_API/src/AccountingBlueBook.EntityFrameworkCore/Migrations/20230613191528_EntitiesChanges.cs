using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountingBlueBook.Migrations
{
    /// <inheritdoc />
    public partial class EntitiesChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Spouses_SpouseId",
                table: "Customers");

            migrationBuilder.DropTable(
                name: "Spouses");

            migrationBuilder.DropIndex(
                name: "IX_Customers_SpouseId",
                table: "Customers");

            migrationBuilder.AlterColumn<long>(
                name: "SpouseId",
                table: "Customers",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "PaymentTermList",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Days = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_PaymentTermList", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentTermList");

            migrationBuilder.AlterColumn<int>(
                name: "SpouseId",
                table: "Customers",
                type: "int",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Spouses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    EthnicityId = table.Column<int>(type: "int", nullable: true),
                    JobTitleId = table.Column<int>(type: "int", nullable: true),
                    LanguageId = table.Column<int>(type: "int", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    DLExpiry = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DLIssue = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DLState = table.Column<int>(type: "int", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DrivingLicense = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SSN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpouseJobDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpouseSuffix = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spouses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Spouses_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Spouses_Ethnicities_EthnicityId",
                        column: x => x.EthnicityId,
                        principalTable: "Ethnicities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Spouses_JobTitles_JobTitleId",
                        column: x => x.JobTitleId,
                        principalTable: "JobTitles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Spouses_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_SpouseId",
                table: "Customers",
                column: "SpouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Spouses_CompanyId",
                table: "Spouses",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Spouses_EthnicityId",
                table: "Spouses",
                column: "EthnicityId");

            migrationBuilder.CreateIndex(
                name: "IX_Spouses_JobTitleId",
                table: "Spouses",
                column: "JobTitleId");

            migrationBuilder.CreateIndex(
                name: "IX_Spouses_LanguageId",
                table: "Spouses",
                column: "LanguageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Spouses_SpouseId",
                table: "Customers",
                column: "SpouseId",
                principalTable: "Spouses",
                principalColumn: "Id");
        }
    }
}

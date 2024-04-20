using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountingBlueBook.Migrations
{
    /// <inheritdoc />
    public partial class coaMaster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChartOfAccountsMasters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountNature = table.Column<int>(type: "int", nullable: false),
                    AccountDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountCode = table.Column<long>(type: "bigint", nullable: false),
                    AccountStatus = table.Column<bool>(type: "bit", nullable: false),
                    Detail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AllowDepreciation = table.Column<bool>(type: "bit", nullable: false),
                    CreditLimitAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsCashFlow = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDefaultAccount = table.Column<bool>(type: "bit", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    AccountTypeId = table.Column<int>(type: "int", nullable: true),
                    MainHeadId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChartOfAccountsMasters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChartOfAccountsMasters_AccountTypes_AccountTypeId",
                        column: x => x.AccountTypeId,
                        principalTable: "AccountTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ChartOfAccountsMasters_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChartOfAccountsMasters_MainHeads_MainHeadId",
                        column: x => x.MainHeadId,
                        principalTable: "MainHeads",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChartOfAccountsMasters_AccountTypeId",
                table: "ChartOfAccountsMasters",
                column: "AccountTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ChartOfAccountsMasters_CompanyId",
                table: "ChartOfAccountsMasters",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ChartOfAccountsMasters_MainHeadId",
                table: "ChartOfAccountsMasters",
                column: "MainHeadId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChartOfAccountsMasters");
        }
    }
}

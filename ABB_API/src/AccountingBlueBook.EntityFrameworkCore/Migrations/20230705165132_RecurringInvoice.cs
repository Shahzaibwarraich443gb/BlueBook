using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountingBlueBook.Migrations
{
    /// <inheritdoc />
    public partial class RecurringInvoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RecurringInvoices",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    DurationId = table.Column<int>(type: "int", nullable: false),
                    FrequencyId = table.Column<int>(type: "int", nullable: false),
                    DurationDateTill = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DurationAmount = table.Column<int>(type: "int", nullable: true),
                    ExecutedAmount = table.Column<int>(type: "int", nullable: true),
                    FrequencyCustomDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FrequencyEveryDayCount = table.Column<int>(type: "int", nullable: true),
                    FrequencyWeek = table.Column<int>(type: "int", nullable: true),
                    FrequencyMonth = table.Column<int>(type: "int", nullable: true),
                    FrequencyAnnualDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InvoiceData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    LastExecution = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SendMail = table.Column<bool>(type: "bit", nullable: false),
                    CustomerCardId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_RecurringInvoices", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecurringInvoices");
        }
    }
}

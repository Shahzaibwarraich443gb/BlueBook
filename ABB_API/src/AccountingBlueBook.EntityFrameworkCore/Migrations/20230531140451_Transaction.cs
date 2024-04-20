using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountingBlueBook.Migrations
{
    /// <inheritdoc />
    public partial class Transaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TransactionNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ref_CompanyID = table.Column<long>(type: "bigint", nullable: true),
                    Ref_CustomerID = table.Column<long>(type: "bigint", nullable: true),
                    TranDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferalNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DebitDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReceiptDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DepositDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ChartOfAccountID = table.Column<long>(type: "bigint", nullable: true),
                    CreditAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TransactionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DebitAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    VerifiedBy = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PaymentReceiveID = table.Column<int>(type: "int", nullable: true),
                    PaymentReceiveNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImportFlag = table.Column<bool>(type: "bit", nullable: true),
                    ImportID = table.Column<int>(type: "int", nullable: true),
                    InvoiceTypeID = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    NoteStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Flaged_ind = table.Column<bool>(type: "bit", nullable: true),
                    Verified_ind = table.Column<bool>(type: "bit", nullable: true),
                    Ref_ReconciliationId = table.Column<bool>(type: "bit", nullable: true),
                    Split_ReconInd = table.Column<bool>(type: "bit", nullable: true),
                    BatchReconInd = table.Column<bool>(type: "bit", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transactions");
        }
    }
}

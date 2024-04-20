using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountingBlueBook.Migrations
{
    /// <inheritdoc />
    public partial class customerSchemaChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Verified_ind",
                table: "Transactions",
                newName: "VerifiedInd");

            migrationBuilder.RenameColumn(
                name: "Split_ReconInd",
                table: "Transactions",
                newName: "SplitReconInd");

            migrationBuilder.RenameColumn(
                name: "Ref_ReconciliationId",
                table: "Transactions",
                newName: "RefReconciliationId");

            migrationBuilder.RenameColumn(
                name: "Ref_CustomerID",
                table: "Transactions",
                newName: "RefCustomerID");

            migrationBuilder.RenameColumn(
                name: "Ref_CompanyID",
                table: "Transactions",
                newName: "RefCompanyID");

            migrationBuilder.RenameColumn(
                name: "Flaged_ind",
                table: "Transactions",
                newName: "FlagedInd");

            migrationBuilder.AddColumn<string>(
                name: "LicenseComment",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LicenseComment",
                table: "Customers");

            migrationBuilder.RenameColumn(
                name: "VerifiedInd",
                table: "Transactions",
                newName: "Verified_ind");

            migrationBuilder.RenameColumn(
                name: "SplitReconInd",
                table: "Transactions",
                newName: "Split_ReconInd");

            migrationBuilder.RenameColumn(
                name: "RefReconciliationId",
                table: "Transactions",
                newName: "Ref_ReconciliationId");

            migrationBuilder.RenameColumn(
                name: "RefCustomerID",
                table: "Transactions",
                newName: "Ref_CustomerID");

            migrationBuilder.RenameColumn(
                name: "RefCompanyID",
                table: "Transactions",
                newName: "Ref_CompanyID");

            migrationBuilder.RenameColumn(
                name: "FlagedInd",
                table: "Transactions",
                newName: "Flaged_ind");
        }
    }
}

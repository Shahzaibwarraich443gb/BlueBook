using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountingBlueBook.Migrations
{
    /// <inheritdoc />
    public partial class multipleDependents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Dependents_DependentId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_DependentId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "DependentId",
                table: "Customers");

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "Dependents",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DependentIds",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dependents_CustomerId",
                table: "Dependents",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dependents_Customers_CustomerId",
                table: "Dependents",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dependents_Customers_CustomerId",
                table: "Dependents");

            migrationBuilder.DropIndex(
                name: "IX_Dependents_CustomerId",
                table: "Dependents");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Dependents");

            migrationBuilder.DropColumn(
                name: "DependentIds",
                table: "Customers");

            migrationBuilder.AddColumn<int>(
                name: "DependentId",
                table: "Customers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_DependentId",
                table: "Customers",
                column: "DependentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Dependents_DependentId",
                table: "Customers",
                column: "DependentId",
                principalTable: "Dependents",
                principalColumn: "Id");
        }
    }
}

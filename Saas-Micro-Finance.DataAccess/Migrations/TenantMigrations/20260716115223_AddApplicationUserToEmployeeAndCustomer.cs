using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saas_Micro_Finance.DataAccess.Migrations.TenantMigrations
{
    /// <inheritdoc />
    public partial class AddApplicationUserToEmployeeAndCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastLoginAt",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<decimal>(
                name: "Interest",
                table: "Loans",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PenaltyRate",
                table: "LoanProducts",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "InterestRate",
                table: "LoanProducts",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Employees",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Customers",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ApplicationUserId",
                table: "Employees",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_ApplicationUserId",
                table: "Customers",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_AspNetUsers_ApplicationUserId",
                table: "Customers",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_AspNetUsers_ApplicationUserId",
                table: "Employees",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_AspNetUsers_ApplicationUserId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_AspNetUsers_ApplicationUserId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_ApplicationUserId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Customers_ApplicationUserId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Customers");

            migrationBuilder.AlterColumn<decimal>(
                name: "Interest",
                table: "Loans",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4);

            migrationBuilder.AlterColumn<decimal>(
                name: "PenaltyRate",
                table: "LoanProducts",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4);

            migrationBuilder.AlterColumn<decimal>(
                name: "InterestRate",
                table: "LoanProducts",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoginAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }
    }
}

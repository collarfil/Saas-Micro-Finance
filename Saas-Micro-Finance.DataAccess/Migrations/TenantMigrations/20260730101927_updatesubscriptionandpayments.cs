using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saas_Micro_Finance.DataAccess.Migrations.TenantMigrations
{
    /// <inheritdoc />
    public partial class updatesubscriptionandpayments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Paystacks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "PaidAt",
                table: "Paystacks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "Paystacks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Flutterwaves",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "PaidAt",
                table: "Flutterwaves",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "Flutterwaves",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Paystacks_SubscriptionId",
                table: "Paystacks",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Flutterwaves_SubscriptionId",
                table: "Flutterwaves",
                column: "SubscriptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Flutterwaves_Subscriptions_SubscriptionId",
                table: "Flutterwaves",
                column: "SubscriptionId",
                principalTable: "Subscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Paystacks_Subscriptions_SubscriptionId",
                table: "Paystacks",
                column: "SubscriptionId",
                principalTable: "Subscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flutterwaves_Subscriptions_SubscriptionId",
                table: "Flutterwaves");

            migrationBuilder.DropForeignKey(
                name: "FK_Paystacks_Subscriptions_SubscriptionId",
                table: "Paystacks");

            migrationBuilder.DropIndex(
                name: "IX_Paystacks_SubscriptionId",
                table: "Paystacks");

            migrationBuilder.DropIndex(
                name: "IX_Flutterwaves_SubscriptionId",
                table: "Flutterwaves");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Paystacks");

            migrationBuilder.DropColumn(
                name: "PaidAt",
                table: "Paystacks");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "Paystacks");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Flutterwaves");

            migrationBuilder.DropColumn(
                name: "PaidAt",
                table: "Flutterwaves");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "Flutterwaves");
        }
    }
}

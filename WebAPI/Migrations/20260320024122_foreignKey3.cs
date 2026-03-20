using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class foreignKey3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contributors_MyOrganizations_MyOrganizationModelMyOrganizationId",
                table: "Contributors");

            migrationBuilder.DropIndex(
                name: "IX_Contributors_MyOrganizationModelMyOrganizationId",
                table: "Contributors");

            migrationBuilder.DropColumn(
                name: "MyOrganizationModelMyOrganizationId",
                table: "Contributors");

            migrationBuilder.CreateIndex(
                name: "IX_Contributors_MyOrganizationId",
                table: "Contributors",
                column: "MyOrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contributors_MyOrganizations_MyOrganizationId",
                table: "Contributors",
                column: "MyOrganizationId",
                principalTable: "MyOrganizations",
                principalColumn: "MyOrganizationId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contributors_MyOrganizations_MyOrganizationId",
                table: "Contributors");

            migrationBuilder.DropIndex(
                name: "IX_Contributors_MyOrganizationId",
                table: "Contributors");

            migrationBuilder.AddColumn<Guid>(
                name: "MyOrganizationModelMyOrganizationId",
                table: "Contributors",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contributors_MyOrganizationModelMyOrganizationId",
                table: "Contributors",
                column: "MyOrganizationModelMyOrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contributors_MyOrganizations_MyOrganizationModelMyOrganizationId",
                table: "Contributors",
                column: "MyOrganizationModelMyOrganizationId",
                principalTable: "MyOrganizations",
                principalColumn: "MyOrganizationId");
        }
    }
}

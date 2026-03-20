using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class followup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contributors",
                columns: table => new
                {
                    ContributorsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MyOrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MyOrganizationModelMyOrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contributors", x => x.ContributorsId);
                    table.ForeignKey(
                        name: "FK_Contributors_MyOrganizations_MyOrganizationModelMyOrganizationId",
                        column: x => x.MyOrganizationModelMyOrganizationId,
                        principalTable: "MyOrganizations",
                        principalColumn: "MyOrganizationId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contributors_MyOrganizationModelMyOrganizationId",
                table: "Contributors",
                column: "MyOrganizationModelMyOrganizationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contributors");
        }
    }
}

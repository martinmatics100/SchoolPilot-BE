using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolPilot.Lookup.Data.Postgres.Core.Migrations
{
    /// <inheritdoc />
    public partial class modifiedStateEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Region",
                table: "States",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Countries",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneCode",
                table: "Countries",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Region",
                table: "Countries",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Subregion",
                table: "Countries",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_States_CountryId",
                table: "States",
                column: "CountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_States_Countries_CountryId",
                table: "States",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_States_Countries_CountryId",
                table: "States");

            migrationBuilder.DropIndex(
                name: "IX_States_CountryId",
                table: "States");

            migrationBuilder.DropColumn(
                name: "Region",
                table: "States");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "PhoneCode",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "Region",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "Subregion",
                table: "Countries");
        }
    }
}

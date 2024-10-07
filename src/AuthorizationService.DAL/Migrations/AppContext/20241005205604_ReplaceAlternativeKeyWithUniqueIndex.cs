using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthorizationService.DAL.Migrations.AppContext
{
    /// <inheritdoc />
    public partial class ReplaceAlternativeKeyWithUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Passports_IdentificationNumber",
                table: "Passports");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Passports_Series_Number",
                table: "Passports");

            migrationBuilder.CreateIndex(
                name: "IX_Passports_IdentificationNumber",
                table: "Passports",
                column: "IdentificationNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Passports_Series_Number",
                table: "Passports",
                columns: new[] { "Series", "Number" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Passports_IdentificationNumber",
                table: "Passports");

            migrationBuilder.DropIndex(
                name: "IX_Passports_Series_Number",
                table: "Passports");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Passports_IdentificationNumber",
                table: "Passports",
                column: "IdentificationNumber");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Passports_Series_Number",
                table: "Passports",
                columns: new[] { "Series", "Number" });
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LetsEncrypt.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Added_AccountPem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountPem",
                table: "CERTIFICATE_ENTRIES",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountPem",
                table: "CERTIFICATE_ENTRIES");
        }
    }
}

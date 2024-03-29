﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LetsEncrypt.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Initial_Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CERTIFICATE_ENTRIES",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CountryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Locality = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Organization = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrganizationUnit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DomainName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsStaging = table.Column<bool>(type: "bit", nullable: false),
                    RenewedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiresOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SysId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CERTIFICATE_ENTRIES", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "LOGGING_ENTRIES",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Level = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SysId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LOGGING_ENTRIES", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CERTIFICATE_ENTRIES_SysId",
                table: "CERTIFICATE_ENTRIES",
                column: "SysId")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_LOGGING_ENTRIES_DateTime",
                table: "LOGGING_ENTRIES",
                column: "DateTime");

            migrationBuilder.CreateIndex(
                name: "IX_LOGGING_ENTRIES_SysId",
                table: "LOGGING_ENTRIES",
                column: "SysId")
                .Annotation("SqlServer:Clustered", true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CERTIFICATE_ENTRIES");

            migrationBuilder.DropTable(
                name: "LOGGING_ENTRIES");
        }
    }
}

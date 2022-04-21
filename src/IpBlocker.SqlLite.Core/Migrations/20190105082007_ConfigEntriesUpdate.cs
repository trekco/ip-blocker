using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IpBlocker.SqlLite.Core.Migrations
{
    public partial class ConfigEntriesUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BlockedIpRecords",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Ip = table.Column<string>(nullable: true),
                    Ports = table.Column<string>(nullable: true),
                    Protocol = table.Column<string>(nullable: true),
                    IsBlocked = table.Column<bool>(nullable: false),
                    IpLocation = table.Column<string>(nullable: true),
                    DateBlocked = table.Column<DateTime>(nullable: false),
                    DateToUnblockIp = table.Column<DateTime>(nullable: false),
                    DateUnblocked = table.Column<DateTime>(nullable: true),
                    Source = table.Column<string>(nullable: true),
                    RuleName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockedIpRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConfigEntries",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigEntries", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlockedIpRecords");

            migrationBuilder.DropTable(
                name: "ConfigEntries");
        }
    }
}

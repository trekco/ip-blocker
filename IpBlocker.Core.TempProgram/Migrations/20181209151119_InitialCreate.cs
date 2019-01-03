using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IpBlocker.Core.TempProgram.Migrations
{
    public partial class InitialCreate : Migration
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
                    IsBlocked = table.Column<bool>(nullable: false),
                    poop = table.Column<bool>(nullable: false),
                    IpLocation = table.Column<string>(nullable: true),
                    DateBlocked = table.Column<DateTime>(nullable: false),
                    DateToUnblockIp = table.Column<DateTime>(nullable: false),
                    DateUnblocked = table.Column<DateTime>(nullable: true),
                    Source = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockedIpRecords", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlockedIpRecords");
        }
    }
}

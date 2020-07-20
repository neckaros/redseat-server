using Microsoft.EntityFrameworkCore.Migrations;

namespace redseat_server.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Downloaders",
                columns: table => new
                {
                    DownloaderId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DownloaderType = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Path = table.Column<string>(type: "TEXT", nullable: true),
                    token = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Downloaders", x => x.DownloaderId);
                });

            migrationBuilder.CreateTable(
                name: "Downloads",
                columns: table => new
                {
                    DownloadId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DownloaderId1 = table.Column<int>(type: "INTEGER", nullable: true),
                    DownloaderId = table.Column<string>(type: "TEXT", nullable: true),
                    DownloadStatus = table.Column<string>(type: "TEXT", nullable: false),
                    Progress = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Downloads", x => x.DownloadId);
                    table.ForeignKey(
                        name: "FK_Downloads_Downloaders_DownloaderId1",
                        column: x => x.DownloaderId1,
                        principalTable: "Downloaders",
                        principalColumn: "DownloaderId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Downloads_DownloaderId1",
                table: "Downloads",
                column: "DownloaderId1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Downloads");

            migrationBuilder.DropTable(
                name: "Downloaders");
        }
    }
}

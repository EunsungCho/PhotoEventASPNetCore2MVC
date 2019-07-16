using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PhotoEventTest.Migrations
{
    public partial class PhotoEventTestAddVoteScore : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    EventId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EventName = table.Column<string>(maxLength: 200, nullable: false),
                    StartDate = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
                    EndDate = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
                    IsClosed = table.Column<bool>(nullable: true, defaultValueSql: "((0))"),
                    Winner = table.Column<string>(maxLength: 50, nullable: true),
                    IntroImage = table.Column<byte[]>(nullable: true),
                    EventRule = table.Column<string>(type: "ntext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tmp_ms_x__7944C810C5678DB0", x => x.EventId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<string>(maxLength: 50, nullable: false),
                    Password = table.Column<string>(maxLength: 50, nullable: false),
                    FirstName = table.Column<string>(maxLength: 50, nullable: true),
                    LastName = table.Column<string>(maxLength: 10, nullable: true),
                    EmailAddress = table.Column<string>(maxLength: 100, nullable: true),
                    IsAdmin = table.Column<bool>(nullable: true, defaultValueSql: "((0))"),
                    EntryDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdateDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Users__1788CC4C5D647209", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "EventUserPhotos",
                columns: table => new
                {
                    EventId = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(maxLength: 50, nullable: false),
                    UserIdToVote = table.Column<string>(maxLength: 50, nullable: true),
                    ThumbnailPhoto = table.Column<byte[]>(nullable: true),
                    Photo = table.Column<byte[]>(nullable: true),
                    VoteScore = table.Column<int>(nullable: false),
                    PhotoUploadDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    PhotoTitle = table.Column<string>(maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Table", x => new { x.EventId, x.UserId });
                    table.ForeignKey(
                        name: "FK_EventUserPhotos_Events",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventUserPhotos_Users",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventUserPhotos_UserId",
                table: "EventUserPhotos",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventUserPhotos");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}

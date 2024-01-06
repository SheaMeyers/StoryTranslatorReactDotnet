using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoryTranslatorReactDotnet.Migrations
{
    /// <inheritdoc />
    public partial class AddUserTranslationParagraph : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserTranslatedParagraphs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ParagraphId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    English = table.Column<string>(type: "text", nullable: false),
                    Spanish = table.Column<string>(type: "text", nullable: false),
                    French = table.Column<string>(type: "text", nullable: false),
                    German = table.Column<string>(type: "text", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTranslatedParagraphs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserTranslatedParagraphs_Paragraphs_ParagraphId",
                        column: x => x.ParagraphId,
                        principalTable: "Paragraphs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserTranslatedParagraphs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserTranslatedParagraphs_ParagraphId",
                table: "UserTranslatedParagraphs",
                column: "ParagraphId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTranslatedParagraphs_UserId",
                table: "UserTranslatedParagraphs",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserTranslatedParagraphs");
        }
    }
}

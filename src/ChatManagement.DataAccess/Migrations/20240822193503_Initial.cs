using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ChatManagement.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserIds = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Chats",
                columns: new[] { "Id", "CreatedAt", "CreatorId", "Title", "UserIds" },
                values: new object[,]
                {
                    { new Guid("67cf8250-220e-4bd7-9d25-4ea480617e49"), new DateTime(2024, 8, 22, 22, 35, 3, 67, DateTimeKind.Local).AddTicks(7563), new Guid("00000000-0000-0000-0000-000000000000"), "Chat 1", "[\"58c58ff5-aeba-43e9-bf88-20fe29ae5ac2\"]" },
                    { new Guid("8ba44e3b-8164-450a-ab84-06e4d6add351"), new DateTime(2024, 8, 22, 22, 35, 3, 67, DateTimeKind.Local).AddTicks(7629), new Guid("00000000-0000-0000-0000-000000000000"), "Chat 2", "[\"dd3ced32-c87e-4146-a423-f5a7e59574b2\"]" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Chats");
        }
    }
}

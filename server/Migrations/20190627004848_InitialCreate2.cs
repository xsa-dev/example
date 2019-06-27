using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Migrations
{
    public partial class InitialCreate2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    isVerified = table.Column<bool>(nullable: false),
                    VerificationCode = table.Column<string>(nullable: true),
                    PasswordHash = table.Column<byte[]>(nullable: true),
                    PasswordSalt = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    fromId = table.Column<int>(nullable: false),
                    toId = table.Column<int>(nullable: false),
                    account = table.Column<int>(nullable: false),
                    amount = table.Column<decimal>(nullable: false),
                    isValidated = table.Column<bool>(nullable: false),
                    date = table.Column<DateTime>(nullable: false),
                    correspondent = table.Column<string>(nullable: true),
                    direction = table.Column<int>(nullable: false),
                    description = table.Column<string>(nullable: true),
                    subtotal = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.id);
                    table.ForeignKey(
                        name: "FK_Transactions_Users_fromId",
                        column: x => x.fromId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Transactions_Users_toId",
                        column: x => x.toId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_fromId",
                table: "Transactions",
                column: "fromId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_toId",
                table: "Transactions",
                column: "toId");


            migrationBuilder.InsertData(
                 table: "Users",
                 columns: new[] { "Username", "Email", "isVerified", "VerificationCode" },
                 values: new object[] { "PW Admin", "info@pw.com", true, "0000" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}

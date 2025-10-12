using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WalletService.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Wallets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RealMoneyInCents = table.Column<long>(type: "bigint", nullable: false),
                    BonusMoneyInCents = table.Column<long>(type: "bigint", nullable: false),
                    LockedFundsInCents = table.Column<long>(type: "bigint", nullable: false),
                    LoyaltyPoints = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallets", x => x.Id);
                    table.CheckConstraint("CK_Wallet_PositiveBalance", "[RealMoneyInCents] >= 0 AND [BonusMoneyInCents] >= 0 AND [LockedFundsInCents] >= 0 AND [LoyaltyPoints] >= 0");
                    table.ForeignKey(
                        name: "FK_Wallets_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AmountInCents = table.Column<long>(type: "bigint", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsCaptured = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    WalletId = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.Id);
                    table.CheckConstraint("CK_Reservation_PositiveAmount", "[AmountInCents] >= 0");
                    table.ForeignKey(
                        name: "FK_Reservations_Wallets_WalletId",
                        column: x => x.WalletId,
                        principalTable: "Wallets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BalanceHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OldBalanceInCents = table.Column<long>(type: "bigint", nullable: false),
                    NewBalanceInCents = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WalletId = table.Column<int>(type: "int", nullable: false),
                    ReservationId = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BalanceHistories", x => x.Id);
                    table.CheckConstraint("CK_BalanceHistory_PositiveBalance", "[OldBalanceInCents] >= 0 AND [NewBalanceInCents] >= 0");
                    table.ForeignKey(
                        name: "FK_BalanceHistories_Reservations_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "Reservations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BalanceHistories_Wallets_WalletId",
                        column: x => x.WalletId,
                        principalTable: "Wallets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReservationTickets",
                columns: table => new
                {
                    ReservationId = table.Column<int>(type: "int", nullable: false),
                    TicketId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationTickets", x => new { x.ReservationId, x.TicketId });
                    table.ForeignKey(
                        name: "FK_ReservationTickets_Reservations_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "Reservations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Players",
                columns: new[] { "Id", "CreatedOn", "DeletedOn", "ModifiedOn", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 10, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Player 1" },
                    { 2, new DateTime(2025, 10, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Player 2 (CPU)" },
                    { 3, new DateTime(2025, 10, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Player 3 (CPU)" },
                    { 4, new DateTime(2025, 10, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Player 4 (CPU)" },
                    { 5, new DateTime(2025, 10, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Player 5 (CPU)" },
                    { 6, new DateTime(2025, 10, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Player 6 (CPU)" },
                    { 7, new DateTime(2025, 10, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Player 7 (CPU)" },
                    { 8, new DateTime(2025, 10, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Player 8 (CPU)" },
                    { 9, new DateTime(2025, 10, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Player 9 (CPU)" },
                    { 10, new DateTime(2025, 10, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Player 10 (CPU)" },
                    { 11, new DateTime(2025, 10, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Player 11 (CPU)" },
                    { 12, new DateTime(2025, 10, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Player 12 (CPU)" },
                    { 13, new DateTime(2025, 10, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Player 13 (CPU)" },
                    { 14, new DateTime(2025, 10, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Player 14 (CPU)" },
                    { 15, new DateTime(2025, 10, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Player 15 (CPU)" }
                });

            migrationBuilder.InsertData(
                table: "Wallets",
                columns: new[] { "Id", "BonusMoneyInCents", "CreatedOn", "DeletedOn", "LockedFundsInCents", "LoyaltyPoints", "ModifiedOn", "PlayerId", "RealMoneyInCents" },
                values: new object[,]
                {
                    { 1, 0L, new DateTime(2025, 10, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 0L, 0, null, 1, 1000L },
                    { 2, 0L, new DateTime(2025, 10, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 0L, 0, null, 2, 1000L },
                    { 3, 0L, new DateTime(2025, 10, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 0L, 0, null, 3, 1000L },
                    { 4, 0L, new DateTime(2025, 10, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 0L, 0, null, 4, 1000L },
                    { 5, 0L, new DateTime(2025, 10, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 0L, 0, null, 5, 1000L },
                    { 6, 0L, new DateTime(2025, 10, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 0L, 0, null, 6, 1000L },
                    { 7, 0L, new DateTime(2025, 10, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 0L, 0, null, 7, 1000L },
                    { 8, 0L, new DateTime(2025, 10, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 0L, 0, null, 8, 1000L },
                    { 9, 0L, new DateTime(2025, 10, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 0L, 0, null, 9, 1000L },
                    { 10, 0L, new DateTime(2025, 10, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 0L, 0, null, 10, 1000L },
                    { 11, 0L, new DateTime(2025, 10, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 0L, 0, null, 11, 1000L },
                    { 12, 0L, new DateTime(2025, 10, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 0L, 0, null, 12, 1000L },
                    { 13, 0L, new DateTime(2025, 10, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 0L, 0, null, 13, 1000L },
                    { 14, 0L, new DateTime(2025, 10, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 0L, 0, null, 14, 1000L },
                    { 15, 0L, new DateTime(2025, 10, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 0L, 0, null, 15, 1000L }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BalanceHistories_ReservationId",
                table: "BalanceHistories",
                column: "ReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_BalanceHistories_WalletId",
                table: "BalanceHistories",
                column: "WalletId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_WalletId",
                table: "Reservations",
                column: "WalletId");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_PlayerId",
                table: "Wallets",
                column: "PlayerId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BalanceHistories");

            migrationBuilder.DropTable(
                name: "ReservationTickets");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "Wallets");

            migrationBuilder.DropTable(
                name: "Players");
        }
    }
}

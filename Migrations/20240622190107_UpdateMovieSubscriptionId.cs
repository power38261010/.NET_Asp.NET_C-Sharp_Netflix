using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetflixClone.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMovieSubscriptionId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MovieSubscription",
                table: "MovieSubscription");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "MovieSubscription",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovieSubscription",
                table: "MovieSubscription",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_MovieSubscription_MovieId",
                table: "MovieSubscription",
                column: "MovieId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MovieSubscription",
                table: "MovieSubscription");

            migrationBuilder.DropIndex(
                name: "IX_MovieSubscription_MovieId",
                table: "MovieSubscription");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "MovieSubscription",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovieSubscription",
                table: "MovieSubscription",
                columns: new[] { "MovieId", "SubscriptionId" });
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace NetflixClone.Migrations
{
    public partial class RecreateMovieSubscriptionId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Renombrar la tabla temporalmente para evitar conflictos
            migrationBuilder.RenameTable(
                name: "MovieSubscription",
                newName: "MovieSubscription_Old");

            // Crear la nueva tabla con la columna 'Id' autogenerada
            migrationBuilder.CreateTable(
                name: "MovieSubscription",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MovieId = table.Column<int>(nullable: false),
                    SubscriptionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieSubscription", x => x.Id);
                });

            // Copiar los datos de la tabla antigua a la nueva
            migrationBuilder.Sql(@"
                INSERT INTO MovieSubscription (MovieId, SubscriptionId)
                SELECT MovieId, SubscriptionId
                FROM MovieSubscription_Old
            ");

            // Eliminar la tabla antigua
            migrationBuilder.DropTable(
                name: "MovieSubscription_Old");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Renombrar la tabla temporalmente para evitar conflictos
            migrationBuilder.RenameTable(
                name: "MovieSubscription",
                newName: "MovieSubscription_New");

            // Volver a crear la tabla antigua
            migrationBuilder.CreateTable(
                name: "MovieSubscription",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MovieId = table.Column<int>(nullable: false),
                    SubscriptionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieSubscription", x => x.Id);
                });

            // Copiar los datos de la tabla nueva a la antigua
            migrationBuilder.Sql(@"
                INSERT INTO MovieSubscription (MovieId, SubscriptionId)
                SELECT MovieId, SubscriptionId
                FROM MovieSubscription_New
            ");

            // Eliminar la tabla nueva
            migrationBuilder.DropTable(
                name: "MovieSubscription_New");
        }
    }
}

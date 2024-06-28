using Microsoft.EntityFrameworkCore.Migrations;

namespace NetflixClone.Migrations
{
    public partial class RecreateMovieSubscriptionId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Renombrar la tabla actual para evitar conflictos
            migrationBuilder.RenameTable(
                name: "MovieSubscription",
                newName: "MovieSubscription_Old");

            // Crear la nueva tabla con la columna 'Id' autogenerada e IDENTITY
            migrationBuilder.CreateTable(
                name: "MovieSubscription",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"), // Esto indica que Id es una columna IDENTITY
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
            // Renombrar la nueva tabla para evitar conflictos
            migrationBuilder.RenameTable(
                name: "MovieSubscription",
                newName: "MovieSubscription_New");

            // Volver a crear la tabla antigua con la estructura original (sin IDENTITY)
            migrationBuilder.CreateTable(
                name: "MovieSubscription",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false), // No especificamos IDENTITY aquí
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

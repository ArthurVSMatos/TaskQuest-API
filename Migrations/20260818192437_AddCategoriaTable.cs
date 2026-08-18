using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskQuest.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoriaTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "multiplicador_xp",
                table: "categorias",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "multiplicador_xp",
                table: "categorias");
        }
    }
}

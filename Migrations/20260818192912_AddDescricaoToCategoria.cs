using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskQuest.API.Migrations
{
    /// <inheritdoc />
    public partial class AddDescricaoToCategoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "descricao",
                table: "categorias",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "descricao",
                table: "categorias");
        }
    }
}

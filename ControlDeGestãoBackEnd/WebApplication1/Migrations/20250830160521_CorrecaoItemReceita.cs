using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class CorrecaoItemReceita : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemReceitas_Produtos_IngredienteId",
                table: "ItemReceitas");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemReceitas_Ingredientes_IngredienteId",
                table: "ItemReceitas",
                column: "IngredienteId",
                principalTable: "Ingredientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemReceitas_Ingredientes_IngredienteId",
                table: "ItemReceitas");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemReceitas_Produtos_IngredienteId",
                table: "ItemReceitas",
                column: "IngredienteId",
                principalTable: "Produtos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

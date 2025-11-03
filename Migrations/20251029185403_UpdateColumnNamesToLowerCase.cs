using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleDespesas.Migrations
{
    /// <inheritdoc />
    public partial class UpdateColumnNamesToLowerCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categorias_Usuarios_UsuarioId",
                table: "Categorias");

            migrationBuilder.DropForeignKey(
                name: "FK_Transacoes_Categorias_CategoriaId",
                table: "Transacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Transacoes_Usuarios_UsuarioId",
                table: "Transacoes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Usuarios",
                table: "Usuarios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Transacoes",
                table: "Transacoes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categorias",
                table: "Categorias");

            migrationBuilder.RenameTable(
                name: "Usuarios",
                newName: "usuarios");

            migrationBuilder.RenameTable(
                name: "Transacoes",
                newName: "transacoes");

            migrationBuilder.RenameTable(
                name: "Categorias",
                newName: "categorias");

            migrationBuilder.RenameColumn(
                name: "Senha",
                table: "usuarios",
                newName: "senha");

            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "usuarios",
                newName: "nome");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "usuarios",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "usuarios",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Valor",
                table: "transacoes",
                newName: "valor");

            migrationBuilder.RenameColumn(
                name: "UsuarioId",
                table: "transacoes",
                newName: "usuarioid");

            migrationBuilder.RenameColumn(
                name: "Tipo",
                table: "transacoes",
                newName: "tipo");

            migrationBuilder.RenameColumn(
                name: "Descricao",
                table: "transacoes",
                newName: "descricao");

            migrationBuilder.RenameColumn(
                name: "Data",
                table: "transacoes",
                newName: "data");

            migrationBuilder.RenameColumn(
                name: "CategoriaId",
                table: "transacoes",
                newName: "categoriaid");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "transacoes",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_Transacoes_UsuarioId",
                table: "transacoes",
                newName: "IX_transacoes_usuarioid");

            migrationBuilder.RenameIndex(
                name: "IX_Transacoes_CategoriaId",
                table: "transacoes",
                newName: "IX_transacoes_categoriaid");

            migrationBuilder.RenameColumn(
                name: "UsuarioId",
                table: "categorias",
                newName: "usuarioid");

            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "categorias",
                newName: "nome");

            migrationBuilder.RenameColumn(
                name: "Descricao",
                table: "categorias",
                newName: "descricao");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "categorias",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_Categorias_UsuarioId",
                table: "categorias",
                newName: "IX_categorias_usuarioid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_usuarios",
                table: "usuarios",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_transacoes",
                table: "transacoes",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_categorias",
                table: "categorias",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_categorias_usuarios_usuarioid",
                table: "categorias",
                column: "usuarioid",
                principalTable: "usuarios",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_transacoes_categorias_categoriaid",
                table: "transacoes",
                column: "categoriaid",
                principalTable: "categorias",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_transacoes_usuarios_usuarioid",
                table: "transacoes",
                column: "usuarioid",
                principalTable: "usuarios",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_categorias_usuarios_usuarioid",
                table: "categorias");

            migrationBuilder.DropForeignKey(
                name: "FK_transacoes_categorias_categoriaid",
                table: "transacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_transacoes_usuarios_usuarioid",
                table: "transacoes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_usuarios",
                table: "usuarios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_transacoes",
                table: "transacoes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_categorias",
                table: "categorias");

            migrationBuilder.RenameTable(
                name: "usuarios",
                newName: "Usuarios");

            migrationBuilder.RenameTable(
                name: "transacoes",
                newName: "Transacoes");

            migrationBuilder.RenameTable(
                name: "categorias",
                newName: "Categorias");

            migrationBuilder.RenameColumn(
                name: "senha",
                table: "Usuarios",
                newName: "Senha");

            migrationBuilder.RenameColumn(
                name: "nome",
                table: "Usuarios",
                newName: "Nome");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "Usuarios",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Usuarios",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "valor",
                table: "Transacoes",
                newName: "Valor");

            migrationBuilder.RenameColumn(
                name: "usuarioid",
                table: "Transacoes",
                newName: "UsuarioId");

            migrationBuilder.RenameColumn(
                name: "tipo",
                table: "Transacoes",
                newName: "Tipo");

            migrationBuilder.RenameColumn(
                name: "descricao",
                table: "Transacoes",
                newName: "Descricao");

            migrationBuilder.RenameColumn(
                name: "data",
                table: "Transacoes",
                newName: "Data");

            migrationBuilder.RenameColumn(
                name: "categoriaid",
                table: "Transacoes",
                newName: "CategoriaId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Transacoes",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_transacoes_usuarioid",
                table: "Transacoes",
                newName: "IX_Transacoes_UsuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_transacoes_categoriaid",
                table: "Transacoes",
                newName: "IX_Transacoes_CategoriaId");

            migrationBuilder.RenameColumn(
                name: "usuarioid",
                table: "Categorias",
                newName: "UsuarioId");

            migrationBuilder.RenameColumn(
                name: "nome",
                table: "Categorias",
                newName: "Nome");

            migrationBuilder.RenameColumn(
                name: "descricao",
                table: "Categorias",
                newName: "Descricao");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Categorias",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_categorias_usuarioid",
                table: "Categorias",
                newName: "IX_Categorias_UsuarioId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Usuarios",
                table: "Usuarios",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Transacoes",
                table: "Transacoes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categorias",
                table: "Categorias",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Categorias_Usuarios_UsuarioId",
                table: "Categorias",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transacoes_Categorias_CategoriaId",
                table: "Transacoes",
                column: "CategoriaId",
                principalTable: "Categorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transacoes_Usuarios_UsuarioId",
                table: "Transacoes",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

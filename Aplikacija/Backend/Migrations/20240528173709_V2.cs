using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class V2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Height",
                table: "DraggableItems",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<int>(
                name: "BrojMesta",
                table: "DraggableItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<double>(
                name: "HeightFactor",
                table: "DraggableItems",
                type: "float",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SurfaceDimensions",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Height = table.Column<double>(type: "float", nullable: false),
                    Width = table.Column<double>(type: "float", nullable: false),
                    SurfaceDimension = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurfaceDimensions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SurfaceDimensions_PlanoviProstora_SurfaceDimension",
                        column: x => x.SurfaceDimension,
                        principalTable: "PlanoviProstora",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SurfaceDimensions_SurfaceDimension",
                table: "SurfaceDimensions",
                column: "SurfaceDimension",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SurfaceDimensions");

            migrationBuilder.DropColumn(
                name: "HeightFactor",
                table: "DraggableItems");

            migrationBuilder.AlterColumn<double>(
                name: "Height",
                table: "DraggableItems",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "BrojMesta",
                table: "DraggableItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}

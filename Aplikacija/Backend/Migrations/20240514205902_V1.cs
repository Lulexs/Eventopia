using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class V1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Korisnici",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Korisnici", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Tagovi",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TagName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tagovi", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Prostori",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Grad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Drzava = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Adresa = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    Slika = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VlasnikProstoraID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prostori", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Prostori_Korisnici_VlasnikProstoraID",
                        column: x => x.VlasnikProstoraID,
                        principalTable: "Korisnici",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "RezervacijeProstora",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VremeOd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VremeDo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ProstorID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RezervacijeProstora", x => x.ID);
                    table.ForeignKey(
                        name: "FK_RezervacijeProstora_Prostori_ProstorID",
                        column: x => x.ProstorID,
                        principalTable: "Prostori",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Dogadjaji",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Vreme = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Slika = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VideoLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RezervacijaProstoraFK = table.Column<int>(type: "int", nullable: false),
                    OrganizatorID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dogadjaji", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Dogadjaji_Korisnici_OrganizatorID",
                        column: x => x.OrganizatorID,
                        principalTable: "Korisnici",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Dogadjaji_RezervacijeProstora_RezervacijaProstoraFK",
                        column: x => x.RezervacijaProstoraFK,
                        principalTable: "RezervacijeProstora",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DogadjajTag",
                columns: table => new
                {
                    DogadjajiID = table.Column<int>(type: "int", nullable: false),
                    TagoviID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DogadjajTag", x => new { x.DogadjajiID, x.TagoviID });
                    table.ForeignKey(
                        name: "FK_DogadjajTag_Dogadjaji_DogadjajiID",
                        column: x => x.DogadjajiID,
                        principalTable: "Dogadjaji",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DogadjajTag_Tagovi_TagoviID",
                        column: x => x.TagoviID,
                        principalTable: "Tagovi",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ocene",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Vrednost = table.Column<int>(type: "int", nullable: false),
                    Komentar = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DogadjajID = table.Column<int>(type: "int", nullable: true),
                    KorisnikID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ocene", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Ocene_Dogadjaji_DogadjajID",
                        column: x => x.DogadjajID,
                        principalTable: "Dogadjaji",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Ocene_Korisnici_KorisnikID",
                        column: x => x.KorisnikID,
                        principalTable: "Korisnici",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "PlanoviProstora",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProstorID = table.Column<int>(type: "int", nullable: false),
                    DogadjajID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanoviProstora", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PlanoviProstora_Dogadjaji_DogadjajID",
                        column: x => x.DogadjajID,
                        principalTable: "Dogadjaji",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_PlanoviProstora_Prostori_ProstorID",
                        column: x => x.ProstorID,
                        principalTable: "Prostori",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DraggableItems",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tip = table.Column<int>(type: "int", nullable: false),
                    Top = table.Column<double>(type: "float", nullable: false),
                    Left = table.Column<double>(type: "float", nullable: false),
                    Height = table.Column<double>(type: "float", nullable: false),
                    BrojMesta = table.Column<int>(type: "int", nullable: false),
                    PlanProstoraID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DraggableItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DraggableItems_PlanoviProstora_PlanProstoraID",
                        column: x => x.PlanProstoraID,
                        principalTable: "PlanoviProstora",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Lines",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    X1 = table.Column<double>(type: "float", nullable: false),
                    X2 = table.Column<double>(type: "float", nullable: false),
                    Y1 = table.Column<double>(type: "float", nullable: false),
                    Y2 = table.Column<double>(type: "float", nullable: false),
                    PlanProstoraID = table.Column<int>(type: "int", nullable: false),
                    Corner1FK = table.Column<int>(type: "int", nullable: true),
                    Corner2FK = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lines", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Lines_DraggableItems_Corner1FK",
                        column: x => x.Corner1FK,
                        principalTable: "DraggableItems",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Lines_DraggableItems_Corner2FK",
                        column: x => x.Corner2FK,
                        principalTable: "DraggableItems",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Lines_PlanoviProstora_PlanProstoraID",
                        column: x => x.PlanProstoraID,
                        principalTable: "PlanoviProstora",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rezervacije",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VremeRezervacije = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BrojMesta = table.Column<int>(type: "int", nullable: false),
                    StoFK = table.Column<int>(type: "int", nullable: true),
                    DogadjajID = table.Column<int>(type: "int", nullable: false),
                    KorisnikID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rezervacije", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Rezervacije_Dogadjaji_DogadjajID",
                        column: x => x.DogadjajID,
                        principalTable: "Dogadjaji",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rezervacije_DraggableItems_StoFK",
                        column: x => x.StoFK,
                        principalTable: "DraggableItems",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Rezervacije_Korisnici_KorisnikID",
                        column: x => x.KorisnikID,
                        principalTable: "Korisnici",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dogadjaji_OrganizatorID",
                table: "Dogadjaji",
                column: "OrganizatorID");

            migrationBuilder.CreateIndex(
                name: "IX_Dogadjaji_RezervacijaProstoraFK",
                table: "Dogadjaji",
                column: "RezervacijaProstoraFK",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DogadjajTag_TagoviID",
                table: "DogadjajTag",
                column: "TagoviID");

            migrationBuilder.CreateIndex(
                name: "IX_DraggableItems_PlanProstoraID",
                table: "DraggableItems",
                column: "PlanProstoraID");

            migrationBuilder.CreateIndex(
                name: "IX_Lines_Corner1FK",
                table: "Lines",
                column: "Corner1FK",
                unique: true,
                filter: "[Corner1FK] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Lines_Corner2FK",
                table: "Lines",
                column: "Corner2FK",
                unique: true,
                filter: "[Corner2FK] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Lines_PlanProstoraID",
                table: "Lines",
                column: "PlanProstoraID");

            migrationBuilder.CreateIndex(
                name: "IX_Ocene_DogadjajID",
                table: "Ocene",
                column: "DogadjajID");

            migrationBuilder.CreateIndex(
                name: "IX_Ocene_KorisnikID",
                table: "Ocene",
                column: "KorisnikID");

            migrationBuilder.CreateIndex(
                name: "IX_PlanoviProstora_DogadjajID",
                table: "PlanoviProstora",
                column: "DogadjajID");

            migrationBuilder.CreateIndex(
                name: "IX_PlanoviProstora_ProstorID",
                table: "PlanoviProstora",
                column: "ProstorID");

            migrationBuilder.CreateIndex(
                name: "IX_Prostori_VlasnikProstoraID",
                table: "Prostori",
                column: "VlasnikProstoraID");

            migrationBuilder.CreateIndex(
                name: "IX_Rezervacije_DogadjajID",
                table: "Rezervacije",
                column: "DogadjajID");

            migrationBuilder.CreateIndex(
                name: "IX_Rezervacije_KorisnikID",
                table: "Rezervacije",
                column: "KorisnikID");

            migrationBuilder.CreateIndex(
                name: "IX_Rezervacije_StoFK",
                table: "Rezervacije",
                column: "StoFK",
                unique: true,
                filter: "[StoFK] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RezervacijeProstora_ProstorID",
                table: "RezervacijeProstora",
                column: "ProstorID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DogadjajTag");

            migrationBuilder.DropTable(
                name: "Lines");

            migrationBuilder.DropTable(
                name: "Ocene");

            migrationBuilder.DropTable(
                name: "Rezervacije");

            migrationBuilder.DropTable(
                name: "Tagovi");

            migrationBuilder.DropTable(
                name: "DraggableItems");

            migrationBuilder.DropTable(
                name: "PlanoviProstora");

            migrationBuilder.DropTable(
                name: "Dogadjaji");

            migrationBuilder.DropTable(
                name: "RezervacijeProstora");

            migrationBuilder.DropTable(
                name: "Prostori");

            migrationBuilder.DropTable(
                name: "Korisnici");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace University_Admissions_Scoring_Engine.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Algorytm",
                columns: table => new
                {
                    IdAlgorytm = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Algorytm", x => x.IdAlgorytm);
                });

            migrationBuilder.CreateTable(
                name: "AlgorytmOperacja",
                columns: table => new
                {
                    IdAlgorytmOperacja = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlgorytmOperacja", x => x.IdAlgorytmOperacja);
                });

            migrationBuilder.CreateTable(
                name: "Kandydat",
                columns: table => new
                {
                    IdKandydat = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Imie = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Nazwisko = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Telefon = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kandydat", x => x.IdKandydat);
                });

            migrationBuilder.CreateTable(
                name: "KierunekRodzaj",
                columns: table => new
                {
                    IdRodzaj = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KierunekRodzaj", x => x.IdRodzaj);
                });

            migrationBuilder.CreateTable(
                name: "KierunekTryb",
                columns: table => new
                {
                    IdTryb = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KierunekTryb", x => x.IdTryb);
                });

            migrationBuilder.CreateTable(
                name: "Matura",
                columns: table => new
                {
                    IdMatura = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SkalaOd = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    SkalaDo = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    SkalaUnit = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matura", x => x.IdMatura);
                });

            migrationBuilder.CreateTable(
                name: "Przedmiot",
                columns: table => new
                {
                    IdPrzedmiot = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Przedmiot", x => x.IdPrzedmiot);
                });

            migrationBuilder.CreateTable(
                name: "PrzedmiotPoziom",
                columns: table => new
                {
                    IdPrzedmiotPoziom = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrzedmiotPoziom", x => x.IdPrzedmiotPoziom);
                });

            migrationBuilder.CreateTable(
                name: "PrzedmiotRodzaj",
                columns: table => new
                {
                    IdPrzedmiotRodzaj = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrzedmiotRodzaj", x => x.IdPrzedmiotRodzaj);
                });

            migrationBuilder.CreateTable(
                name: "Status",
                columns: table => new
                {
                    IdStatus = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Status", x => x.IdStatus);
                });

            migrationBuilder.CreateTable(
                name: "Kierunek",
                columns: table => new
                {
                    IdKierunek = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TrybId = table.Column<int>(type: "int", nullable: false),
                    RodzajId = table.Column<int>(type: "int", nullable: false),
                    MinPrzyjetych = table.Column<int>(type: "int", nullable: false),
                    MaxPrzyjetych = table.Column<int>(type: "int", nullable: false),
                    MaxListaRezerwowa = table.Column<int>(type: "int", nullable: false),
                    AlgorytmId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kierunek", x => x.IdKierunek);
                    table.ForeignKey(
                        name: "FK_Kierunek_Algorytm_AlgorytmId",
                        column: x => x.AlgorytmId,
                        principalTable: "Algorytm",
                        principalColumn: "IdAlgorytm",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Kierunek_KierunekRodzaj_RodzajId",
                        column: x => x.RodzajId,
                        principalTable: "KierunekRodzaj",
                        principalColumn: "IdRodzaj",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Kierunek_KierunekTryb_TrybId",
                        column: x => x.TrybId,
                        principalTable: "KierunekTryb",
                        principalColumn: "IdTryb",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AlgorytmMatura",
                columns: table => new
                {
                    IdAlgorytmMatura = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlgorytmId = table.Column<int>(type: "int", nullable: false),
                    MaturaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlgorytmMatura", x => x.IdAlgorytmMatura);
                    table.ForeignKey(
                        name: "FK_AlgorytmMatura_Algorytm_AlgorytmId",
                        column: x => x.AlgorytmId,
                        principalTable: "Algorytm",
                        principalColumn: "IdAlgorytm",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlgorytmMatura_Matura_MaturaId",
                        column: x => x.MaturaId,
                        principalTable: "Matura",
                        principalColumn: "IdMatura",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KandydatDyplom",
                columns: table => new
                {
                    IdKandydatDyplom = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KandydatId = table.Column<int>(type: "int", nullable: false),
                    MaturaId = table.Column<int>(type: "int", nullable: false),
                    Numer = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Rok = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KandydatDyplom", x => x.IdKandydatDyplom);
                    table.ForeignKey(
                        name: "FK_KandydatDyplom_Kandydat_KandydatId",
                        column: x => x.KandydatId,
                        principalTable: "Kandydat",
                        principalColumn: "IdKandydat",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KandydatDyplom_Matura_MaturaId",
                        column: x => x.MaturaId,
                        principalTable: "Matura",
                        principalColumn: "IdMatura",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Przedmiot_Rodzaj_Poziom",
                columns: table => new
                {
                    IdPrzedmiotRodzajPoziom = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrzedmiotId = table.Column<int>(type: "int", nullable: false),
                    PrzedmiotRodzajId = table.Column<int>(type: "int", nullable: false),
                    PrzedmiotPoziomId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Przedmiot_Rodzaj_Poziom", x => x.IdPrzedmiotRodzajPoziom);
                    table.ForeignKey(
                        name: "FK_Przedmiot_Rodzaj_Poziom_PrzedmiotPoziom_PrzedmiotPoziomId",
                        column: x => x.PrzedmiotPoziomId,
                        principalTable: "PrzedmiotPoziom",
                        principalColumn: "IdPrzedmiotPoziom",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Przedmiot_Rodzaj_Poziom_PrzedmiotRodzaj_PrzedmiotRodzajId",
                        column: x => x.PrzedmiotRodzajId,
                        principalTable: "PrzedmiotRodzaj",
                        principalColumn: "IdPrzedmiotRodzaj",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Przedmiot_Rodzaj_Poziom_Przedmiot_PrzedmiotId",
                        column: x => x.PrzedmiotId,
                        principalTable: "Przedmiot",
                        principalColumn: "IdPrzedmiot",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Kandydat_Kierunek",
                columns: table => new
                {
                    IdKandydatKierunek = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KandydatId = table.Column<int>(type: "int", nullable: false),
                    KierunekId = table.Column<int>(type: "int", nullable: false),
                    Punkty = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Ranking = table.Column<int>(type: "int", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kandydat_Kierunek", x => x.IdKandydatKierunek);
                    table.ForeignKey(
                        name: "FK_Kandydat_Kierunek_Kandydat_KandydatId",
                        column: x => x.KandydatId,
                        principalTable: "Kandydat",
                        principalColumn: "IdKandydat",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Kandydat_Kierunek_Kierunek_KierunekId",
                        column: x => x.KierunekId,
                        principalTable: "Kierunek",
                        principalColumn: "IdKierunek",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Kandydat_Kierunek_Status_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Status",
                        principalColumn: "IdStatus",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AlgorytmGrupa",
                columns: table => new
                {
                    IdAlgorytmGrupa = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlgorytmMaturaId = table.Column<int>(type: "int", nullable: false),
                    RodzicId = table.Column<int>(type: "int", nullable: true),
                    AlgorytmOperacjaId = table.Column<int>(type: "int", nullable: false),
                    AlgorytmOperacjaIdAlgorytmOperacja = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlgorytmGrupa", x => x.IdAlgorytmGrupa);
                    table.ForeignKey(
                        name: "FK_AlgorytmGrupa_AlgorytmGrupa_RodzicId",
                        column: x => x.RodzicId,
                        principalTable: "AlgorytmGrupa",
                        principalColumn: "IdAlgorytmGrupa",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AlgorytmGrupa_AlgorytmMatura_AlgorytmMaturaId",
                        column: x => x.AlgorytmMaturaId,
                        principalTable: "AlgorytmMatura",
                        principalColumn: "IdAlgorytmMatura",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlgorytmGrupa_AlgorytmOperacja_AlgorytmOperacjaId",
                        column: x => x.AlgorytmOperacjaId,
                        principalTable: "AlgorytmOperacja",
                        principalColumn: "IdAlgorytmOperacja",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AlgorytmGrupa_AlgorytmOperacja_AlgorytmOperacjaIdAlgorytmOperacja",
                        column: x => x.AlgorytmOperacjaIdAlgorytmOperacja,
                        principalTable: "AlgorytmOperacja",
                        principalColumn: "IdAlgorytmOperacja");
                });

            migrationBuilder.CreateTable(
                name: "KandydatDyplom_Przedmiot",
                columns: table => new
                {
                    IdKandydatDyplomPrzedmiot = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KandydatDyplomId = table.Column<int>(type: "int", nullable: false),
                    PrzedmiotRodzajPoziomId = table.Column<int>(type: "int", nullable: false),
                    Punkty = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KandydatDyplom_Przedmiot", x => x.IdKandydatDyplomPrzedmiot);
                    table.ForeignKey(
                        name: "FK_KandydatDyplom_Przedmiot_KandydatDyplom_KandydatDyplomId",
                        column: x => x.KandydatDyplomId,
                        principalTable: "KandydatDyplom",
                        principalColumn: "IdKandydatDyplom",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KandydatDyplom_Przedmiot_Przedmiot_Rodzaj_Poziom_PrzedmiotRodzajPoziomId",
                        column: x => x.PrzedmiotRodzajPoziomId,
                        principalTable: "Przedmiot_Rodzaj_Poziom",
                        principalColumn: "IdPrzedmiotRodzajPoziom",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Matura_Przedmiot",
                columns: table => new
                {
                    IdMaturaPrzedmiot = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaturaId = table.Column<int>(type: "int", nullable: false),
                    PrzedmiotRodzajPoziomId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matura_Przedmiot", x => x.IdMaturaPrzedmiot);
                    table.ForeignKey(
                        name: "FK_Matura_Przedmiot_Matura_MaturaId",
                        column: x => x.MaturaId,
                        principalTable: "Matura",
                        principalColumn: "IdMatura",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Matura_Przedmiot_Przedmiot_Rodzaj_Poziom_PrzedmiotRodzajPoziomId",
                        column: x => x.PrzedmiotRodzajPoziomId,
                        principalTable: "Przedmiot_Rodzaj_Poziom",
                        principalColumn: "IdPrzedmiotRodzajPoziom",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AlgorytmLicz",
                columns: table => new
                {
                    IdAlgorytmLicz = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrzedmiotRodzajPoziomId = table.Column<int>(type: "int", nullable: false),
                    AlgorytmGrupaId = table.Column<int>(type: "int", nullable: false),
                    Liczba = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlgorytmLicz", x => x.IdAlgorytmLicz);
                    table.ForeignKey(
                        name: "FK_AlgorytmLicz_AlgorytmGrupa_AlgorytmGrupaId",
                        column: x => x.AlgorytmGrupaId,
                        principalTable: "AlgorytmGrupa",
                        principalColumn: "IdAlgorytmGrupa",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlgorytmLicz_Przedmiot_Rodzaj_Poziom_PrzedmiotRodzajPoziomId",
                        column: x => x.PrzedmiotRodzajPoziomId,
                        principalTable: "Przedmiot_Rodzaj_Poziom",
                        principalColumn: "IdPrzedmiotRodzajPoziom",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlgorytmGrupa_AlgorytmMaturaId",
                table: "AlgorytmGrupa",
                column: "AlgorytmMaturaId");

            migrationBuilder.CreateIndex(
                name: "IX_AlgorytmGrupa_AlgorytmOperacjaId",
                table: "AlgorytmGrupa",
                column: "AlgorytmOperacjaId");

            migrationBuilder.CreateIndex(
                name: "IX_AlgorytmGrupa_AlgorytmOperacjaIdAlgorytmOperacja",
                table: "AlgorytmGrupa",
                column: "AlgorytmOperacjaIdAlgorytmOperacja");

            migrationBuilder.CreateIndex(
                name: "IX_AlgorytmGrupa_RodzicId",
                table: "AlgorytmGrupa",
                column: "RodzicId");

            migrationBuilder.CreateIndex(
                name: "IX_AlgorytmLicz_AlgorytmGrupaId",
                table: "AlgorytmLicz",
                column: "AlgorytmGrupaId");

            migrationBuilder.CreateIndex(
                name: "IX_AlgorytmLicz_PrzedmiotRodzajPoziomId",
                table: "AlgorytmLicz",
                column: "PrzedmiotRodzajPoziomId");

            migrationBuilder.CreateIndex(
                name: "IX_AlgorytmMatura_AlgorytmId_MaturaId",
                table: "AlgorytmMatura",
                columns: new[] { "AlgorytmId", "MaturaId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AlgorytmMatura_MaturaId",
                table: "AlgorytmMatura",
                column: "MaturaId");

            migrationBuilder.CreateIndex(
                name: "IX_Kandydat_Email",
                table: "Kandydat",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kandydat_Kierunek_KandydatId_KierunekId",
                table: "Kandydat_Kierunek",
                columns: new[] { "KandydatId", "KierunekId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kandydat_Kierunek_KierunekId",
                table: "Kandydat_Kierunek",
                column: "KierunekId");

            migrationBuilder.CreateIndex(
                name: "IX_Kandydat_Kierunek_StatusId",
                table: "Kandydat_Kierunek",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_KandydatDyplom_KandydatId",
                table: "KandydatDyplom",
                column: "KandydatId");

            migrationBuilder.CreateIndex(
                name: "IX_KandydatDyplom_MaturaId",
                table: "KandydatDyplom",
                column: "MaturaId");

            migrationBuilder.CreateIndex(
                name: "IX_KandydatDyplom_Numer",
                table: "KandydatDyplom",
                column: "Numer",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KandydatDyplom_Przedmiot_KandydatDyplomId_PrzedmiotRodzajPoziomId",
                table: "KandydatDyplom_Przedmiot",
                columns: new[] { "KandydatDyplomId", "PrzedmiotRodzajPoziomId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KandydatDyplom_Przedmiot_PrzedmiotRodzajPoziomId",
                table: "KandydatDyplom_Przedmiot",
                column: "PrzedmiotRodzajPoziomId");

            migrationBuilder.CreateIndex(
                name: "IX_Kierunek_AlgorytmId",
                table: "Kierunek",
                column: "AlgorytmId");

            migrationBuilder.CreateIndex(
                name: "IX_Kierunek_RodzajId",
                table: "Kierunek",
                column: "RodzajId");

            migrationBuilder.CreateIndex(
                name: "IX_Kierunek_TrybId",
                table: "Kierunek",
                column: "TrybId");

            migrationBuilder.CreateIndex(
                name: "IX_Matura_Przedmiot_MaturaId_PrzedmiotRodzajPoziomId",
                table: "Matura_Przedmiot",
                columns: new[] { "MaturaId", "PrzedmiotRodzajPoziomId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Matura_Przedmiot_PrzedmiotRodzajPoziomId",
                table: "Matura_Przedmiot",
                column: "PrzedmiotRodzajPoziomId");

            migrationBuilder.CreateIndex(
                name: "IX_Przedmiot_Rodzaj_Poziom_PrzedmiotId_PrzedmiotRodzajId_PrzedmiotPoziomId",
                table: "Przedmiot_Rodzaj_Poziom",
                columns: new[] { "PrzedmiotId", "PrzedmiotRodzajId", "PrzedmiotPoziomId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Przedmiot_Rodzaj_Poziom_PrzedmiotPoziomId",
                table: "Przedmiot_Rodzaj_Poziom",
                column: "PrzedmiotPoziomId");

            migrationBuilder.CreateIndex(
                name: "IX_Przedmiot_Rodzaj_Poziom_PrzedmiotRodzajId",
                table: "Przedmiot_Rodzaj_Poziom",
                column: "PrzedmiotRodzajId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlgorytmLicz");

            migrationBuilder.DropTable(
                name: "Kandydat_Kierunek");

            migrationBuilder.DropTable(
                name: "KandydatDyplom_Przedmiot");

            migrationBuilder.DropTable(
                name: "Matura_Przedmiot");

            migrationBuilder.DropTable(
                name: "AlgorytmGrupa");

            migrationBuilder.DropTable(
                name: "Kierunek");

            migrationBuilder.DropTable(
                name: "Status");

            migrationBuilder.DropTable(
                name: "KandydatDyplom");

            migrationBuilder.DropTable(
                name: "Przedmiot_Rodzaj_Poziom");

            migrationBuilder.DropTable(
                name: "AlgorytmMatura");

            migrationBuilder.DropTable(
                name: "AlgorytmOperacja");

            migrationBuilder.DropTable(
                name: "KierunekRodzaj");

            migrationBuilder.DropTable(
                name: "KierunekTryb");

            migrationBuilder.DropTable(
                name: "Kandydat");

            migrationBuilder.DropTable(
                name: "PrzedmiotPoziom");

            migrationBuilder.DropTable(
                name: "PrzedmiotRodzaj");

            migrationBuilder.DropTable(
                name: "Przedmiot");

            migrationBuilder.DropTable(
                name: "Algorytm");

            migrationBuilder.DropTable(
                name: "Matura");
        }
    }
}

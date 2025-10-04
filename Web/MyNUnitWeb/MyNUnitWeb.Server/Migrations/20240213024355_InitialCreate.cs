using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyNUnitWeb.Server.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Assemblies",
                columns: table => new
                {
                    AssemblyResultId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AssemblyName = table.Column<string>(type: "TEXT", nullable: false),
                    PassedTests = table.Column<int>(type: "INTEGER", nullable: false),  
                    FailedTests = table.Column<int>(type: "INTEGER", nullable: false),  
                    IgnoredTests = table.Column<int>(type: "INTEGER", nullable: false)  
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assemblies", x => x.AssemblyResultId);
                });

            migrationBuilder.CreateTable(
                name: "Classes",
                columns: table => new
                {
                    ClassResultId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClassName = table.Column<string>(type: "TEXT", nullable: false),
                    ErrorReason = table.Column<string>(type: "TEXT", nullable: true),  
                    AssemblyResultId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classes", x => x.ClassResultId);
                    table.ForeignKey(
                        name: "FK_Classes_Assemblies_AssemblyResultId",
                        column: x => x.AssemblyResultId,
                        principalTable: "Assemblies",
                        principalColumn: "AssemblyResultId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Methods",
                columns: table => new
                {
                    MethodResultId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MethodName = table.Column<string>(type: "TEXT", nullable: false),
                    TestStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    ErrorReason = table.Column<string>(type: "TEXT", nullable: true),
                    ExpectedResult = table.Column<string>(type: "TEXT", nullable: true),
                    ActualResult = table.Column<string>(type: "TEXT", nullable: true),
                    TimeElapsed = table.Column<long>(type: "INTEGER", nullable: false),
                    ClassResultId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Methods", x => x.MethodResultId);
                    table.ForeignKey(
                        name: "FK_Methods_Classes_ClassResultId",
                        column: x => x.ClassResultId,
                        principalTable: "Classes",
                        principalColumn: "ClassResultId",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateIndex(
                name: "IX_Classes_AssemblyResultId",
                table: "Classes",
                column: "AssemblyResultId");

            migrationBuilder.CreateIndex(
                name: "IX_Methods_ClassResultId",
                table: "Methods",
                column: "ClassResultId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Methods");

            migrationBuilder.DropTable(
                name: "Classes");

            migrationBuilder.DropTable(
                name: "Assemblies");
        }
    }
}

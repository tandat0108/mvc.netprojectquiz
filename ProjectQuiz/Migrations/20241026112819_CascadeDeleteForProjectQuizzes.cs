using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectQuiz.Migrations
{
    /// <inheritdoc />
    public partial class CascadeDeleteForProjectQuizzes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__ProjectMe__Proje__4BAC3F29",
                table: "ProjectMembers");

            migrationBuilder.DropForeignKey(
                name: "FK__ProjectMe__UserI__4CA06362",
                table: "ProjectMembers");

            migrationBuilder.DropForeignKey(
                name: "FK__QuizResul__QuizI__59063A47",
                table: "QuizResults");

            migrationBuilder.DropForeignKey(
                name: "FK__QuizResul__UserI__5812160E",
                table: "QuizResults");

            migrationBuilder.DropForeignKey(
                name: "FK__Quizzes__Project__3C69FB99",
                table: "Quizzes");

            migrationBuilder.DropPrimaryKey(
                name: "PK__QuizResu__3214EC07FB98EAE4",
                table: "QuizResults");

            migrationBuilder.DropPrimaryKey(
                name: "PK__ProjectM__3214EC07BE80DD9D",
                table: "ProjectMembers");

            migrationBuilder.AlterColumn<string>(
                name: "IsCorrect",
                table: "Quizzes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Answers",
                table: "Quizzes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "CorrectAnswer",
                table: "Quizzes",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "QuizResults",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "QuizId",
                table: "QuizResults",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "QuizResults",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "SelectedAnswer",
                table: "QuizResults",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "ProjectMembers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "ProjectMembers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK__QuizResu__3214EC077AC4B913",
                table: "QuizResults",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectMembers",
                table: "ProjectMembers",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "UserQuiz",
                columns: table => new
                {
                    UserQuizId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    TotalScore = table.Column<decimal>(type: "decimal(5,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UserQuiz__20FA63870534A927", x => x.UserQuizId);
                    table.ForeignKey(
                        name: "FK__UserQuiz__Projec__2BFE89A6",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__UserQuiz__UserId__2B0A656D",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserQuiz_ProjectId",
                table: "UserQuiz",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_UserQuiz_UserId",
                table: "UserQuiz",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectMembers_Projects",
                table: "ProjectMembers",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectMembers_Users",
                table: "ProjectMembers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__QuizResul__QuizI__282DF8C2",
                table: "QuizResults",
                column: "QuizId",
                principalTable: "Quizzes",
                principalColumn: "QuizId");

            migrationBuilder.AddForeignKey(
                name: "FK__QuizResul__UserI__2739D489",
                table: "QuizResults",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__Quizzes__Project__3C69FB99",
                table: "Quizzes",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMembers_Projects",
                table: "ProjectMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMembers_Users",
                table: "ProjectMembers");

            migrationBuilder.DropForeignKey(
                name: "FK__QuizResul__QuizI__282DF8C2",
                table: "QuizResults");

            migrationBuilder.DropForeignKey(
                name: "FK__QuizResul__UserI__2739D489",
                table: "QuizResults");

            migrationBuilder.DropForeignKey(
                name: "FK__Quizzes__Project__3C69FB99",
                table: "Quizzes");

            migrationBuilder.DropTable(
                name: "UserQuiz");

            migrationBuilder.DropPrimaryKey(
                name: "PK__QuizResu__3214EC077AC4B913",
                table: "QuizResults");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectMembers",
                table: "ProjectMembers");

            migrationBuilder.DropColumn(
                name: "CorrectAnswer",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "SelectedAnswer",
                table: "QuizResults");

            migrationBuilder.AlterColumn<int>(
                name: "IsCorrect",
                table: "Quizzes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Answers",
                table: "Quizzes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "QuizResults",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "QuizId",
                table: "QuizResults",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "QuizResults",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "ProjectMembers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "ProjectMembers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK__QuizResu__3214EC07FB98EAE4",
                table: "QuizResults",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__ProjectM__3214EC07BE80DD9D",
                table: "ProjectMembers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__ProjectMe__Proje__4BAC3F29",
                table: "ProjectMembers",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__ProjectMe__UserI__4CA06362",
                table: "ProjectMembers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__QuizResul__QuizI__59063A47",
                table: "QuizResults",
                column: "QuizId",
                principalTable: "Quizzes",
                principalColumn: "QuizId");

            migrationBuilder.AddForeignKey(
                name: "FK__QuizResul__UserI__5812160E",
                table: "QuizResults",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__Quizzes__Project__3C69FB99",
                table: "Quizzes",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");
        }
    }
}

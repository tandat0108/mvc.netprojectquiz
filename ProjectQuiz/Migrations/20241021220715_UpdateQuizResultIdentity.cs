using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectQuiz.Migrations
{
    /// <inheritdoc />
    public partial class UpdateQuizResultIdentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Tạo bảng tạm thời để lưu trữ dữ liệu hiện tại
            migrationBuilder.Sql(
                @"CREATE TABLE QuizResults_Temp (
                TempId INT NOT NULL, 
                UserId INT, 
                QuizId INT, 
                Score DECIMAL(5, 2), 
                Comment TEXT
            )");

            // 2. Sao chép dữ liệu từ bảng QuizResults sang bảng tạm thời
            migrationBuilder.Sql("INSERT INTO QuizResults_Temp (TempId, UserId, QuizId, Score, Comment) SELECT Id, UserId, QuizId, Score, Comment FROM QuizResults");

            // 3. Xóa bảng QuizResults
            migrationBuilder.DropTable(name: "QuizResults");

            // 4. Tạo lại bảng QuizResults với cột Id tự động tăng (IDENTITY)
            migrationBuilder.CreateTable(
                name: "QuizResults",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: true),
                    QuizId = table.Column<int>(nullable: true),
                    Score = table.Column<decimal>(type: "decimal(5, 2)", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizResults", x => x.Id);
                });

            // 5. Sao chép dữ liệu từ bảng tạm trở lại bảng QuizResults
            migrationBuilder.Sql("SET IDENTITY_INSERT QuizResults ON");
            migrationBuilder.Sql("INSERT INTO QuizResults (Id, UserId, QuizId, Score, Comment) SELECT TempId, UserId, QuizId, Score, Comment FROM QuizResults_Temp");
            migrationBuilder.Sql("SET IDENTITY_INSERT QuizResults OFF");

            // 6. Xóa bảng tạm
            migrationBuilder.Sql("DROP TABLE QuizResults_Temp");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Nếu cần rollback, có thể đảo ngược lại quá trình như sau:
            // - Tạo lại bảng QuizResults cũ và di chuyển dữ liệu lại
        }
    }



}

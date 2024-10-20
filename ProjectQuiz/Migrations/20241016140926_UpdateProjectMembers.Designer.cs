﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProjectQuiz.Data;

#nullable disable

namespace ProjectQuiz.Migrations
{
    [DbContext(typeof(QuizzDbContext))]
    [Migration("20241016140926_UpdateProjectMembers")]
    partial class UpdateProjectMembers
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ProjectQuiz.Data.Project", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("Description")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("IntroductionVideoUrl")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<DateTime?>("LastUpdatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("Name")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Status")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id")
                        .HasName("PK__Projects__3214EC07FF35E7D5");

                    b.HasIndex("UserId");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("ProjectQuiz.Data.ProjectComment", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Comment")
                        .HasColumnType("text");

                    b.Property<int?>("ProjectId")
                        .HasColumnType("int");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id")
                        .HasName("PK__ProjectC__3214EC07A9B61990");

                    b.HasIndex("ProjectId");

                    b.HasIndex("UserId");

                    b.ToTable("ProjectComments");
                });

            modelBuilder.Entity("ProjectQuiz.Data.ProjectMember", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int?>("ProjectId")
                        .HasColumnType("int");

                    b.Property<string>("Role")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id")
                        .HasName("PK__ProjectM__3214EC07BE80DD9D");

                    b.HasIndex("ProjectId");

                    b.HasIndex("UserId");

                    b.ToTable("ProjectMembers");
                });

            modelBuilder.Entity("ProjectQuiz.Data.Quiz", b =>
                {
                    b.Property<int>("QuizId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("QuizId"));

                    b.Property<string>("Answers")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("IsCorrect")
                        .HasColumnType("int");

                    b.Property<int?>("ProjectId")
                        .HasColumnType("int");

                    b.Property<string>("QuestionText")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("QuestionType")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("QuizId")
                        .HasName("PK__Quizzes__3214EC07AF39F804");

                    b.HasIndex("ProjectId");

                    b.ToTable("Quizzes");
                });

            modelBuilder.Entity("ProjectQuiz.Data.QuizResult", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Comment")
                        .HasColumnType("text");

                    b.Property<int?>("QuizId")
                        .HasColumnType("int");

                    b.Property<decimal?>("Score")
                        .HasColumnType("decimal(5, 2)");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id")
                        .HasName("PK__QuizResu__3214EC07FB98EAE4");

                    b.HasIndex("QuizId");

                    b.HasIndex("UserId");

                    b.ToTable("QuizResults");
                });

            modelBuilder.Entity("ProjectQuiz.Data.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("PasswordHash")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Role")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Username")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id")
                        .HasName("PK__Users__3214EC074064DE7D");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ProjectQuiz.Data.Project", b =>
                {
                    b.HasOne("ProjectQuiz.Data.User", "User")
                        .WithMany("Projects")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__Projects__UserId__398D8EEE");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ProjectQuiz.Data.ProjectComment", b =>
                {
                    b.HasOne("ProjectQuiz.Data.Project", "Project")
                        .WithMany("ProjectComments")
                        .HasForeignKey("ProjectId")
                        .HasConstraintName("FK__ProjectCo__Proje__5BE2A6F2");

                    b.HasOne("ProjectQuiz.Data.User", "User")
                        .WithMany("ProjectComments")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__ProjectCo__UserI__5CD6CB2B");

                    b.Navigation("Project");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ProjectQuiz.Data.ProjectMember", b =>
                {
                    b.HasOne("ProjectQuiz.Data.Project", "Project")
                        .WithMany("ProjectMembers")
                        .HasForeignKey("ProjectId")
                        .HasConstraintName("FK__ProjectMe__Proje__4BAC3F29");

                    b.HasOne("ProjectQuiz.Data.User", "User")
                        .WithMany("ProjectMembers")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__ProjectMe__UserI__4CA06362");

                    b.Navigation("Project");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ProjectQuiz.Data.Quiz", b =>
                {
                    b.HasOne("ProjectQuiz.Data.Project", "Project")
                        .WithMany("Quizzes")
                        .HasForeignKey("ProjectId")
                        .HasConstraintName("FK__Quizzes__Project__3C69FB99");

                    b.Navigation("Project");
                });

            modelBuilder.Entity("ProjectQuiz.Data.QuizResult", b =>
                {
                    b.HasOne("ProjectQuiz.Data.Quiz", "Quiz")
                        .WithMany("QuizResults")
                        .HasForeignKey("QuizId")
                        .HasConstraintName("FK__QuizResul__QuizI__59063A47");

                    b.HasOne("ProjectQuiz.Data.User", "User")
                        .WithMany("QuizResults")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__QuizResul__UserI__5812160E");

                    b.Navigation("Quiz");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ProjectQuiz.Data.Project", b =>
                {
                    b.Navigation("ProjectComments");

                    b.Navigation("ProjectMembers");

                    b.Navigation("Quizzes");
                });

            modelBuilder.Entity("ProjectQuiz.Data.Quiz", b =>
                {
                    b.Navigation("QuizResults");
                });

            modelBuilder.Entity("ProjectQuiz.Data.User", b =>
                {
                    b.Navigation("ProjectComments");

                    b.Navigation("ProjectMembers");

                    b.Navigation("Projects");

                    b.Navigation("QuizResults");
                });
#pragma warning restore 612, 618
        }
    }
}

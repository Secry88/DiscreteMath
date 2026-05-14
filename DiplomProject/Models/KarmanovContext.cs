using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DiplomProject.Models;

public partial class KarmanovContext : DbContext
{
    public KarmanovContext()
    {
    }

    public KarmanovContext(DbContextOptions<KarmanovContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Answer> Answers { get; set; }

    public virtual DbSet<EulerProblem> EulerProblems { get; set; }

    public virtual DbSet<EulerRegion> EulerRegions { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<RegionIdentificationElement> RegionIdentificationElements { get; set; }

    public virtual DbSet<RegionIdentificationTask> RegionIdentificationTasks { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SetOperationCorrectRegion> SetOperationCorrectRegions { get; set; }

    public virtual DbSet<SetOperationStep> SetOperationSteps { get; set; }

    public virtual DbSet<SetOperationTask> SetOperationTasks { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<Test> Tests { get; set; }

    public virtual DbSet<TestResult> TestResults { get; set; }

    public virtual DbSet<TheoryCategory> TheoryCategories { get; set; }

    public virtual DbSet<TheoryContent> TheoryContents { get; set; }

    public virtual DbSet<TheoryTopic> TheoryTopics { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserTestSession> UserTestSessions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=ngknn.ru;Port=5442;Database=Karmanov;Username=21P;Password=123");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("C");

        modelBuilder.Entity<Answer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("answers_pkey");

            entity.ToTable("answers", "Diplom");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AnswerText).HasColumnName("answer_text");
            entity.Property(e => e.Explanation).HasColumnName("explanation");
            entity.Property(e => e.IsCorrect)
                .HasDefaultValue(false)
                .HasColumnName("is_correct");
            entity.Property(e => e.OrderIndex)
                .HasDefaultValue(0)
                .HasColumnName("order_index");
            entity.Property(e => e.QuestionId).HasColumnName("question_id");

            entity.HasOne(d => d.Question).WithMany(p => p.Answers)
                .HasForeignKey(d => d.QuestionId)
                .HasConstraintName("fk_answer_question");
        });

        modelBuilder.Entity<EulerProblem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("euler_problems_pkey");

            entity.ToTable("euler_problems", "Diplom");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DiagramType).HasColumnName("diagram_type");
            entity.Property(e => e.Difficulty)
                .HasDefaultValue(1)
                .HasColumnName("difficulty");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");
        });

        modelBuilder.Entity<EulerRegion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("euler_regions_pkey");

            entity.ToTable("euler_regions", "Diplom");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsCorrect).HasColumnName("is_correct");
            entity.Property(e => e.ProblemId).HasColumnName("problem_id");
            entity.Property(e => e.RegionCode)
                .HasMaxLength(50)
                .HasColumnName("region_code");

            entity.HasOne(d => d.Problem).WithMany(p => p.EulerRegions)
                .HasForeignKey(d => d.ProblemId)
                .HasConstraintName("fk_region_problem");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("groups_pkey");

            entity.ToTable("groups", "Diplom");

            entity.HasIndex(e => e.Name, "groups_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("questions_pkey");

            entity.ToTable("questions", "Diplom");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.OrderIndex)
                .HasDefaultValue(0)
                .HasColumnName("order_index");
            entity.Property(e => e.QuestionText).HasColumnName("question_text");
            entity.Property(e => e.TestId).HasColumnName("test_id");

            entity.HasOne(d => d.Test).WithMany(p => p.Questions)
                .HasForeignKey(d => d.TestId)
                .HasConstraintName("fk_question_test");
        });

        modelBuilder.Entity<RegionIdentificationElement>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("region_identification_elements_pkey");

            entity.ToTable("region_identification_elements", "Diplom");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CorrectRegionNumber)
                .HasDefaultValue(1)
                .HasColumnName("correct_region_number");
            entity.Property(e => e.ElementValue).HasColumnName("element_value");
            entity.Property(e => e.TaskId).HasColumnName("task_id");

            entity.HasOne(d => d.Task).WithMany(p => p.RegionIdentificationElements)
                .HasForeignKey(d => d.TaskId)
                .HasConstraintName("fk_element_task");
        });

        modelBuilder.Entity<RegionIdentificationTask>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("region_identification_tasks_pkey");

            entity.ToTable("region_identification_tasks", "Diplom");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DiagramImage).HasColumnName("diagram_image");
            entity.Property(e => e.DiagramType)
                .HasDefaultValue(2)
                .HasColumnName("diagram_type");
            entity.Property(e => e.Difficulty)
                .HasDefaultValue(1)
                .HasColumnName("difficulty");
            entity.Property(e => e.SetA).HasColumnName("set_a");
            entity.Property(e => e.SetB).HasColumnName("set_b");
            entity.Property(e => e.SetC).HasColumnName("set_c");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");
            entity.Property(e => e.UniversalSet).HasColumnName("universal_set");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("roles_pkey");

            entity.ToTable("roles", "Diplom");

            entity.HasIndex(e => e.Name, "roles_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<SetOperationCorrectRegion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("set_operation_correct_regions_pkey");

            entity.ToTable("set_operation_correct_regions", "Diplom");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.RegionCode)
                .HasMaxLength(50)
                .HasColumnName("region_code");
            entity.Property(e => e.TaskId).HasColumnName("task_id");

            entity.HasOne(d => d.Task).WithMany(p => p.SetOperationCorrectRegions)
                .HasForeignKey(d => d.TaskId)
                .HasConstraintName("fk_correct_region_task");
        });

        modelBuilder.Entity<SetOperationStep>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("set_operation_steps_pkey");

            entity.ToTable("set_operation_steps", "Diplom");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CorrectAnswer).HasColumnName("correct_answer");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Expression)
                .HasMaxLength(200)
                .HasColumnName("expression");
            entity.Property(e => e.StepNumber)
                .HasDefaultValue(0)
                .HasColumnName("step_number");
            entity.Property(e => e.TaskId).HasColumnName("task_id");

            entity.HasOne(d => d.Task).WithMany(p => p.SetOperationSteps)
                .HasForeignKey(d => d.TaskId)
                .HasConstraintName("fk_step_task");
        });

        modelBuilder.Entity<SetOperationTask>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("set_operation_tasks_pkey");

            entity.ToTable("set_operation_tasks", "Diplom");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DiagramType)
                .HasDefaultValue(2)
                .HasColumnName("diagram_type");
            entity.Property(e => e.Difficulty)
                .HasDefaultValue(1)
                .HasColumnName("difficulty");
            entity.Property(e => e.Expression)
                .HasMaxLength(200)
                .HasColumnName("expression");
            entity.Property(e => e.SetA).HasColumnName("set_a");
            entity.Property(e => e.SetB).HasColumnName("set_b");
            entity.Property(e => e.SetC).HasColumnName("set_c");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");
            entity.Property(e => e.UniversalSet).HasColumnName("universal_set");
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tasks_pkey");

            entity.ToTable("tasks", "Diplom");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Condition).HasColumnName("condition");
            entity.Property(e => e.CorrectAnswer).HasColumnName("correct_answer");
            entity.Property(e => e.Operation)
                .HasMaxLength(20)
                .HasColumnName("operation");
            entity.Property(e => e.SetA).HasColumnName("set_a");
            entity.Property(e => e.SetB).HasColumnName("set_b");
            entity.Property(e => e.Subtype).HasColumnName("subtype");
            entity.Property(e => e.Type)
                .HasMaxLength(10)
                .HasColumnName("type");
        });

        modelBuilder.Entity<Test>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tests_pkey");

            entity.ToTable("tests", "Diplom");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Difficulty)
                .HasDefaultValue(1)
                .HasColumnName("difficulty");
            entity.Property(e => e.Duration)
                .HasDefaultValue(10)
                .HasColumnName("duration");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");
            entity.Property(e => e.Topic)
                .HasMaxLength(200)
                .HasColumnName("topic");
        });

        modelBuilder.Entity<TestResult>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("test_results_pkey");

            entity.ToTable("test_results", "Diplom");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AttemptNumber)
                .HasDefaultValue(1)
                .HasColumnName("attempt_number");
            entity.Property(e => e.CompletedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("completed_at");
            entity.Property(e => e.Percentage).HasColumnName("percentage");
            entity.Property(e => e.Score).HasColumnName("score");
            entity.Property(e => e.TestId).HasColumnName("test_id");
            entity.Property(e => e.TotalQuestions).HasColumnName("total_questions");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Test).WithMany(p => p.TestResults)
                .HasForeignKey(d => d.TestId)
                .HasConstraintName("fk_result_test");

            entity.HasOne(d => d.User).WithMany(p => p.TestResults)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_result_user");
        });

        modelBuilder.Entity<TheoryCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("theory_categories_pkey");

            entity.ToTable("theory_categories", "Diplom");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");
        });

        modelBuilder.Entity<TheoryContent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("theory_content_pkey");

            entity.ToTable("theory_content", "Diplom");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.OrderIndex)
                .HasDefaultValue(0)
                .HasColumnName("order_index");
            entity.Property(e => e.TopicId).HasColumnName("topic_id");

            entity.HasOne(d => d.Topic).WithMany(p => p.TheoryContents)
                .HasForeignKey(d => d.TopicId)
                .HasConstraintName("fk_content_topic");
        });

        modelBuilder.Entity<TheoryTopic>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("theory_topics_pkey");

            entity.ToTable("theory_topics", "Diplom");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.OrderIndex)
                .HasDefaultValue(0)
                .HasColumnName("order_index");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");

            entity.HasOne(d => d.Category).WithMany(p => p.TheoryTopics)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("fk_topics_category");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users", "Diplom");

            entity.HasIndex(e => e.Login, "users_login_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FullName)
                .HasMaxLength(150)
                .HasColumnName("full_name");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.Login)
                .HasMaxLength(50)
                .HasColumnName("login");
            entity.Property(e => e.Password).HasColumnName("password");
            entity.Property(e => e.ProfileImage).HasColumnName("profile_image");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Group).WithMany(p => p.Users)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_user_group");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_user_role");
        });

        modelBuilder.Entity<UserTestSession>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_test_sessions_pkey");

            entity.ToTable("user_test_sessions", "Diplom");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CurrentQuestion)
                .HasDefaultValue(0)
                .HasColumnName("current_question");
            entity.Property(e => e.IsCompleted)
                .HasDefaultValue(false)
                .HasColumnName("is_completed");
            entity.Property(e => e.SelectedAnswers)
                .HasDefaultValueSql("'[]'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("selected_answers");
            entity.Property(e => e.StartedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("started_at");
            entity.Property(e => e.TestId).HasColumnName("test_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Test).WithMany(p => p.UserTestSessions)
                .HasForeignKey(d => d.TestId)
                .HasConstraintName("user_test_sessions_test_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.UserTestSessions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("user_test_sessions_user_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

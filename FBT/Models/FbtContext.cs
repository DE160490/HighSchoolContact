using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FBT.Models;

public partial class FbtContext : DbContext
{
    public FbtContext()
    {
    }

    public FbtContext(DbContextOptions<FbtContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Class> Classes { get; set; }

    //public virtual DbSet<TakeClass> TakeClasses { get; set; }

    //public virtual DbSet<TeachingSubject> TeachingSubjects { get; set; }

    public virtual DbSet<ContactBook> ContactBooks { get; set; }

    public virtual DbSet<EvaluateEducationalOutcome> EvaluateEducationalOutcomes { get; set; }

    public virtual DbSet<Grade> Grades { get; set; }

    public virtual DbSet<LearningOutcome> LearningOutcomes { get; set; }

    public virtual DbSet<Parent> Parents { get; set; }

    public virtual DbSet<PersonInformation> PersonInformations { get; set; }

    public virtual DbSet<ResultOfEvaluation> ResultOfEvaluations { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<SchoolProfile> SchoolProfiles { get; set; }

    public virtual DbSet<SchoolYear> SchoolYears { get; set; }

    public virtual DbSet<Score> Scores { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    //public virtual DbSet<HomeroomTeacher> HomeroomTeachers { get; set; }

    public virtual DbSet<SubjectTeacher> SubjectTeachers { get; set; }

    public virtual DbSet<Teacher> Teachers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-3J5AMMQ;uid=sa;pwd=123456789;database=FBT;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__Account__349DA586B0B1E72E");

            entity.ToTable("Account");

            entity.Property(e => e.AccountId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("AccountID");
            entity.Property(e => e.Password).HasMaxLength(40);

            entity.HasOne(d => d.AccountNavigation).WithOne(p => p.Account)
                .HasForeignKey<Account>(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AccountID");
        });

        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.ClassId).HasName("PK__Class__CB1927A0D95B157F");

            entity.ToTable("Class");

            entity.Property(e => e.ClassId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ClassID");
            entity.Property(e => e.ClassName).HasMaxLength(30);
            entity.Property(e => e.GradeId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("GradeID");

            entity.HasOne(d => d.Grade).WithMany(p => p.Classes)
                .HasForeignKey(d => d.GradeId)
                .HasConstraintName("FK_ClassBelongTo");

            entity.HasMany(d => d.Students).WithMany(p => p.Classes)
                .UsingEntity<Dictionary<string, object>>(
                    "TakeClass",
                    r => r.HasOne<Student>().WithMany()
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ClassOf"),
                    l => l.HasOne<Class>().WithMany()
                        .HasForeignKey("ClassId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_StudentTakeClass"),
                    j =>
                    {
                        j.HasKey("ClassId", "StudentId").HasName("PK__TakeClas__48357507DCEB2DCD");
                        j.ToTable("TakeClass");
                        j.IndexerProperty<string>("ClassId")
                            .HasMaxLength(10)
                            .IsUnicode(false)
                            .HasColumnName("ClassID");
                        j.IndexerProperty<string>("StudentId")
                            .HasMaxLength(10)
                            .IsUnicode(false)
                            .HasColumnName("StudentID");
                    });
        });

        modelBuilder.Entity<ContactBook>(entity =>
        {
            entity.HasKey(e => e.ContactBookId).HasName("PK__ContactB__848C521B928C0138");

            entity.ToTable("ContactBook");

            entity.Property(e => e.ContactBookId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ContactBookID");
            entity.Property(e => e.StudentId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("StudentID");

            entity.HasOne(d => d.Student).WithMany(p => p.ContactBooks)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK_ContactBook");
        });

        modelBuilder.Entity<EvaluateEducationalOutcome>(entity =>
        {
            entity.HasKey(e => new { e.SchoolProfileId, e.Semester, e.AbilitiesAndQualities }).HasName("PK__Evaluate__851265C679E1840B");

            entity.Property(e => e.SchoolProfileId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SchoolProfileID");
            entity.Property(e => e.Semester).HasMaxLength(10);
            entity.Property(e => e.AbilitiesAndQualities).HasMaxLength(30);
            entity.Property(e => e.Level).HasMaxLength(10);

            entity.HasOne(d => d.SchoolProfile).WithMany(p => p.EvaluateEducationalOutcomes)
                .HasForeignKey(d => d.SchoolProfileId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EvaluateEducationalOutcomesOf");
        });

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.GradeId).HasName("PK__Grade__54F87A3706996592");

            entity.ToTable("Grade");

            entity.Property(e => e.GradeId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("GradeID");
            entity.Property(e => e.GradeName).HasMaxLength(30);
            entity.Property(e => e.SchoolYearId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SchoolYearID");

            entity.HasOne(d => d.SchoolYear).WithMany(p => p.Grades)
                .HasForeignKey(d => d.SchoolYearId)
                .HasConstraintName("FK_GradeBelongTo");
        });

        modelBuilder.Entity<LearningOutcome>(entity =>
        {
            entity.HasKey(e => new { e.SchoolProfileId, e.SubjectId }).HasName("PK__Learning__D8A3B4C2E9CDA340");

            entity.Property(e => e.SchoolProfileId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SchoolProfileID");
            entity.Property(e => e.SubjectId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SubjectID");
            entity.Property(e => e.FinalScores).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Semester1Scores).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Semester2Scores).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.SchoolProfile).WithMany(p => p.LearningOutcomes)
                .HasForeignKey(d => d.SchoolProfileId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LearningOutcomesOf");

            entity.HasOne(d => d.Subject).WithMany(p => p.LearningOutcomes)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LearningOutcomesOfSubject");
        });

        modelBuilder.Entity<Parent>(entity =>
        {
            entity.HasKey(e => new { e.ParentId, e.StudentId }).HasName("PK__Parent__501503A8AF5B9FB4");

            entity.ToTable("Parent");

            entity.Property(e => e.ParentId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ParentID");
            entity.Property(e => e.StudentId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("StudentID");
            entity.Property(e => e.Job).HasMaxLength(120);

            entity.HasOne(d => d.ParentNavigation).WithMany(p => p.Parents)
                .HasForeignKey(d => d.ParentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ParentInf");

            entity.HasOne(d => d.Student).WithMany(p => p.Parents)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ParentOf");
        });

        modelBuilder.Entity<PersonInformation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PersonIn__3214EC27AB0CF12C");

            entity.ToTable("PersonInformation");

            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ID");
            entity.Property(e => e.Dob)
                .HasColumnType("date")
                .HasColumnName("DOB");
            entity.Property(e => e.Email).HasMaxLength(120);
            entity.Property(e => e.Ethnic).HasMaxLength(60);
            entity.Property(e => e.Fullname).HasMaxLength(120);
            entity.Property(e => e.Phone)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.PlaceOfBirth).HasMaxLength(320);
            entity.Property(e => e.PlaceOfResidence).HasMaxLength(320);
            entity.Property(e => e.Religion).HasMaxLength(60);
        });

        modelBuilder.Entity<ResultOfEvaluation>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ResultOfEvaluation");

            entity.Property(e => e.FinalEvaluation).HasMaxLength(20);
            entity.Property(e => e.SchoolProfileId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SchoolProfileID");
            entity.Property(e => e.Semester1Evaluation).HasMaxLength(20);
            entity.Property(e => e.Semester2Evaluation).HasMaxLength(20);

            entity.HasOne(d => d.SchoolProfile).WithMany()
                .HasForeignKey(d => d.SchoolProfileId)
                .HasConstraintName("FK_ResultOfEvaluationOf");
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__Schedule__9C8A5B6917DA5579");

            entity.ToTable("Schedule");

            entity.Property(e => e.ScheduleId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ScheduleID");
            entity.Property(e => e.ClassId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ClassID");
            entity.Property(e => e.DayOfWeek).HasColumnType("date");
            entity.Property(e => e.Lecture).HasMaxLength(20);
            entity.Property(e => e.SubjectId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SubjectID");
            entity.Property(e => e.TeacherId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("TeacherID");
            entity.Property(e => e.WeekBegins).HasColumnType("date");
            entity.Property(e => e.WeekEnds).HasColumnType("date");

            entity.HasOne(d => d.Class).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK_ScheduleSubjectTeacherOfClass");

            entity.HasOne(d => d.Subject).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.SubjectId)
                .HasConstraintName("FK_ScheduleSubjectTeacherOfSubject");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("FK_ScheduleSubjectTeacher");
        });

        modelBuilder.Entity<SchoolProfile>(entity =>
        {
            entity.HasKey(e => e.SchoolProfileId).HasName("PK__SchoolPr__42620EFAB7C00116");

            entity.ToTable("SchoolProfile");

            entity.Property(e => e.SchoolProfileId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SchoolProfileID");
            entity.Property(e => e.StudentId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("StudentID");

            entity.HasOne(d => d.Student).WithMany(p => p.SchoolProfiles)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK_SchoolProfileOf");
        });

        modelBuilder.Entity<SchoolYear>(entity =>
        {
            entity.HasKey(e => e.SchoolYearId).HasName("PK__SchoolYe__9BAABCC07ACADAF0");

            entity.ToTable("SchoolYear");

            entity.Property(e => e.SchoolYearId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SchoolYearID");
            entity.Property(e => e.DateEnd).HasColumnType("date");
            entity.Property(e => e.DateStart).HasColumnType("date");
        });

        modelBuilder.Entity<Score>(entity =>
        {
            entity.HasKey(e => new { e.StudentId, e.SubjectId, e.Semester }).HasName("PK__Score__8F8235E9A5C4C562");

            entity.ToTable("Score");

            entity.Property(e => e.StudentId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("StudentID");
            entity.Property(e => e.SubjectId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SubjectID");
            entity.Property(e => e.Coefficient1).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Coefficient2).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Coefficient3).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.Student).WithMany(p => p.Scores)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ScoreStudent");

            entity.HasOne(d => d.Subject).WithMany(p => p.Scores)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ScoreSubject");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__Student__32C52A79A591E709");

            entity.ToTable("Student");

            entity.Property(e => e.StudentId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("StudentID");

            entity.HasOne(d => d.StudentNavigation).WithOne(p => p.Student)
                .HasForeignKey<Student>(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StudentInf");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.SubjectId).HasName("PK__Subject__AC1BA3889868D421");

            entity.ToTable("Subject");

            entity.Property(e => e.SubjectId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SubjectID");
            entity.Property(e => e.Description).HasMaxLength(120);
            entity.Property(e => e.SubjectName).HasMaxLength(30);
        });

        modelBuilder.Entity<SubjectTeacher>(entity =>
        {
            entity.HasKey(e => new { e.TeacherId, e.ClassId, e.SubjectId }).HasName("PK__SubjectT__78EFD09DF0F21888");

            entity.ToTable("SubjectTeacher");

            entity.Property(e => e.TeacherId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("TeacherID");
            entity.Property(e => e.ClassId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ClassID");
            entity.Property(e => e.SubjectId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SubjectID");

            entity.HasOne(d => d.Class).WithMany(p => p.SubjectTeachers)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SubjectTeacherOfClass");

            entity.HasOne(d => d.Subject).WithMany(p => p.SubjectTeachers)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SubjectTeacherOfSubject");

            entity.HasOne(d => d.Teacher).WithMany(p => p.SubjectTeachers)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SubjectTeacher");
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasKey(e => e.TeacherId).HasName("PK__Teacher__EDF2594401BC81AA");

            entity.ToTable("Teacher");

            entity.Property(e => e.TeacherId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("TeacherID");
            entity.Property(e => e.MainExpertise)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Position).HasMaxLength(60);

            entity.HasOne(d => d.MainExpertiseNavigation).WithMany(p => p.Teachers)
                .HasForeignKey(d => d.MainExpertise)
                .HasConstraintName("FK_MainExpertiseOf");

            entity.HasOne(d => d.TeacherNavigation).WithOne(p => p.Teacher)
                .HasForeignKey<Teacher>(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TeacherInf");

            entity.HasMany(d => d.Classes).WithMany(p => p.Teachers)
                .UsingEntity<Dictionary<string, object>>(
                    "HomeroomTeacher",
                    r => r.HasOne<Class>().WithMany()
                        .HasForeignKey("ClassId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_HomeroomTeacherOfClass"),
                    l => l.HasOne<Teacher>().WithMany()
                        .HasForeignKey("TeacherId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_HomeroomTeacher"),
                    j =>
                    {
                        j.HasKey("TeacherId", "ClassId").HasName("PK__Homeroom__F143CB3E962830B4");
                        j.ToTable("HomeroomTeacher");
                        j.IndexerProperty<string>("TeacherId")
                            .HasMaxLength(10)
                            .IsUnicode(false)
                            .HasColumnName("TeacherID");
                        j.IndexerProperty<string>("ClassId")
                            .HasMaxLength(10)
                            .IsUnicode(false)
                            .HasColumnName("ClassID");
                    });

            entity.HasMany(d => d.Subjects).WithMany(p => p.TeachersNavigation)
                .UsingEntity<Dictionary<string, object>>(
                    "TeachingSubject",
                    r => r.HasOne<Subject>().WithMany()
                        .HasForeignKey("SubjectId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_TeachingSubject"),
                    l => l.HasOne<Teacher>().WithMany()
                        .HasForeignKey("TeacherId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_TeacherStudy"),
                    j =>
                    {
                        j.HasKey("TeacherId", "SubjectId").HasName("PK__Teaching__7733E37C8AB94CE2");
                        j.ToTable("TeachingSubject");
                        j.IndexerProperty<string>("TeacherId")
                            .HasMaxLength(10)
                            .IsUnicode(false)
                            .HasColumnName("TeacherID");
                        j.IndexerProperty<string>("SubjectId")
                            .HasMaxLength(10)
                            .IsUnicode(false)
                            .HasColumnName("SubjectID");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FinalProject.Backend.Data.Entities;

namespace FinalProject.Backend.Data.DbContext;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // --- Core domain ---
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<Class> Classes => Set<Class>();
    public DbSet<ClassEnrollment> ClassEnrollments => Set<ClassEnrollment>();
    public DbSet<ClassSubject> ClassSubjects => Set<ClassSubject>();

    // --- Assignment flow ---
    public DbSet<Assignment> Assignments => Set<Assignment>();
    public DbSet<AssignmentAttachment> AssignmentAttachments => Set<AssignmentAttachment>();
    public DbSet<Submission> Submissions => Set<Submission>();
    public DbSet<SubmissionFile> SubmissionFiles => Set<SubmissionFile>();
    public DbSet<Grade> Grades => Set<Grade>();
    public DbSet<AIAnalysisResult> AIAnalysisResults => Set<AIAnalysisResult>();

    // --- Users & linking ---
    public DbSet<ParentStudentLink> ParentStudentLinks => Set<ParentStudentLink>();

    // --- Support ---
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<OtpCode> OtpCodes => Set<OtpCode>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder); // must call first for Identity tables

        // ---------------------------------------------------------------
        // SOFT DELETE — Global Query Filters
        // Every query on these tables automatically adds WHERE IsDeleted = 0
        // Use .IgnoreQueryFilters() when you need to see deleted records.
        // ---------------------------------------------------------------
        builder.Entity<Subject>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Class>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<ClassEnrollment>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<ClassSubject>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Assignment>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<AssignmentAttachment>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Submission>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<SubmissionFile>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Grade>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<AIAnalysisResult>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<ParentStudentLink>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Notification>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<ApplicationUser>().HasQueryFilter(x => !x.IsDeleted);

        // ---------------------------------------------------------------
        // ApplicationUser — unique constraints
        // ---------------------------------------------------------------
        builder.Entity<ApplicationUser>()
            .HasIndex(u => u.StudentLinkCode)
            .IsUnique()
            .HasFilter("[StudentLinkCode] IS NOT NULL");

        // ---------------------------------------------------------------
        // ClassEnrollment — one user per class (no duplicates)
        // ---------------------------------------------------------------
        builder.Entity<ClassEnrollment>()
            .HasIndex(e => new { e.ClassId, e.UserId })
            .IsUnique();

        builder.Entity<ClassEnrollment>()
            .HasOne(e => e.Class)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(e => e.ClassId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ClassEnrollment>()
            .HasOne(e => e.User)
            .WithMany(u => u.ClassEnrollments)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // ---------------------------------------------------------------
        // ClassSubject — one teacher per subject per class
        // ---------------------------------------------------------------
        builder.Entity<ClassSubject>()
            .HasIndex(cs => new { cs.ClassId, cs.SubjectId })
            .IsUnique();

        builder.Entity<ClassSubject>()
            .HasOne(cs => cs.Teacher)
            .WithMany()
            .HasForeignKey(cs => cs.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        // ---------------------------------------------------------------
        // Assignment
        // ---------------------------------------------------------------
        builder.Entity<Assignment>()
            .HasOne(a => a.CreatedByTeacher)
            .WithMany(u => u.Assignments)
            .HasForeignKey(a => a.CreatedByTeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Assignment>()
            .Property(a => a.Deadline)
            .HasColumnType("datetime2");

        // ---------------------------------------------------------------
        // Submission — one submission per student per assignment
        // ---------------------------------------------------------------
        builder.Entity<Submission>()
            .HasIndex(s => new { s.AssignmentId, s.StudentId })
            .IsUnique();

        builder.Entity<Submission>()
            .HasOne(s => s.Student)
            .WithMany(u => u.Submissions)
            .HasForeignKey(s => s.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Submission>()
            .Property(s => s.Status)
            .HasConversion<string>();

        // ---------------------------------------------------------------
        // Grade — one grade per submission
        // ---------------------------------------------------------------
        builder.Entity<Grade>()
            .HasOne(g => g.Submission)
            .WithOne(s => s.Grade)
            .HasForeignKey<Grade>(g => g.SubmissionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Grade>()
            .HasOne(g => g.GradedByTeacher)
            .WithMany()
            .HasForeignKey(g => g.GradedByTeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Grade>()
            .Property(g => g.Score)
            .HasColumnType("decimal(5,2)");

        // ---------------------------------------------------------------
        // AIAnalysisResult — one result per submission
        // ---------------------------------------------------------------
        builder.Entity<AIAnalysisResult>()
            .HasOne(ai => ai.Submission)
            .WithOne(s => s.AIAnalysis)
            .HasForeignKey<AIAnalysisResult>(ai => ai.SubmissionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<AIAnalysisResult>()
            .Property(ai => ai.AIUsagePercent)
            .HasColumnType("decimal(5,2)");

        builder.Entity<AIAnalysisResult>()
            .Property(ai => ai.SuggestedScore)
            .HasColumnType("decimal(5,2)");

        // ---------------------------------------------------------------
        // ParentStudentLink
        // ---------------------------------------------------------------
        builder.Entity<ParentStudentLink>()
            .HasIndex(l => new { l.ParentId, l.StudentId })
            .IsUnique();

        builder.Entity<ParentStudentLink>()
            .HasOne(l => l.Parent)
            .WithMany(u => u.ParentLinks)
            .HasForeignKey(l => l.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ParentStudentLink>()
            .HasOne(l => l.Student)
            .WithMany(u => u.StudentLinks)
            .HasForeignKey(l => l.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ParentStudentLink>()
            .Property(l => l.Status)
            .HasConversion<string>();

        // ---------------------------------------------------------------
        // OtpCode — not soft-deleted (Hangfire cleans it physically)
        // ---------------------------------------------------------------
        builder.Entity<OtpCode>()
            .HasIndex(o => o.Email);

        builder.Entity<OtpCode>()
            .Property(o => o.ExpiresAt)
            .HasColumnType("datetime2");

        // ---------------------------------------------------------------
        // AuditLog — append-only, no soft delete, long Id for volume
        // ---------------------------------------------------------------
        builder.Entity<AuditLog>()
            .HasOne(al => al.User)
            .WithMany(u => u.AuditLogs)
            .HasForeignKey(al => al.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<AuditLog>()
            .HasIndex(al => new { al.EntityName, al.EntityId });

        builder.Entity<AuditLog>()
            .HasIndex(al => al.Timestamp);
    }
}

using System;
using System.Collections.Generic;

namespace FinalProject.Backend.Data.Entities;

/// <summary>
/// FR-14: A classroom managed by Admin (Lớp học).
/// One class can have many teachers and many students via ClassEnrollments.
/// </summary>
public class Class : BaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    /// <summary>Niên khóa — e.g. "2024-2025"</summary>
    public string AcademicYear { get; set; } = string.Empty;

    public string? Description { get; set; }

    // --- Navigation ---
    public ICollection<ClassEnrollment> Enrollments { get; set; } = [];
    public ICollection<ClassSubject> ClassSubjects { get; set; } = [];
    public ICollection<Assignment> Assignments { get; set; } = [];
}

/// <summary>
/// FR-15: Join table — maps a user (Teacher or Student) to a class.
/// Role in class is determined by the user's Identity role.
/// </summary>
public class ClassEnrollment : BaseEntity
{
    public int Id { get; set; }

    public int ClassId { get; set; }
    public Class Class { get; set; } = null!;

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Which subjects are taught in a class.
/// </summary>
public class ClassSubject : BaseEntity
{
    public int Id { get; set; }

    public int ClassId { get; set; }
    public Class Class { get; set; } = null!;

    public int SubjectId { get; set; }
    public Subject Subject { get; set; } = null!;

    /// <summary>The teacher assigned to this subject in this class.</summary>
    public string TeacherId { get; set; } = string.Empty;
    public ApplicationUser Teacher { get; set; } = null!;
}

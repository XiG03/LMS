using System;
using System.Collections.Generic;

namespace FinalProject.Backend.Data.Entities;

/// <summary>
/// FR-06: Assignment created by a Teacher for a Class + Subject.
/// </summary>
public class Assignment : BaseEntity
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime Deadline { get; set; }

    /// <summary>
    /// FR-06: If true, students can submit after deadline — submission is marked Late.
    /// If false, the Submit button is disabled once Deadline passes.
    /// </summary>
    public bool AllowLateSubmission { get; set; } = false;

    // --- Relationships ---
    public int ClassId { get; set; }
    public Class Class { get; set; } = null!;

    public int SubjectId { get; set; }
    public Subject Subject { get; set; } = null!;

    /// <summary>Teacher who created this assignment.</summary>
    public string CreatedByTeacherId { get; set; } = string.Empty;
    public ApplicationUser CreatedByTeacher { get; set; } = null!;

    // --- Navigation ---
    public ICollection<AssignmentAttachment> Attachments { get; set; } = [];
    public ICollection<Submission> Submissions { get; set; } = [];
}

/// <summary>
/// File(s) attached to an Assignment by the Teacher.
/// Stored on local disk — only the path is saved in DB.
/// </summary>
public class AssignmentAttachment : BaseEntity
{
    public int Id { get; set; }

    public int AssignmentId { get; set; }
    public Assignment Assignment { get; set; } = null!;

    /// <summary>Original file name shown to students.</summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>Relative path on server disk — e.g. /uploads/assignments/3/report.pdf</summary>
    public string FilePath { get; set; } = string.Empty;

    public long FileSizeBytes { get; set; }
    public string ContentType { get; set; } = string.Empty;
}

using System;
using System.Collections.Generic;

namespace FinalProject.Backend.Data.Entities;

public enum SubmissionStatus
{
    OnTime,
    Late,
    NotSubmitted
}

/// <summary>
/// FR-08: A Student's submission for an Assignment.
/// Created when Student hits Submit — triggers AI analysis + SignalR to Teacher.
/// </summary>
public class Submission : BaseEntity
{
    public int Id { get; set; }

    public int AssignmentId { get; set; }
    public Assignment Assignment { get; set; } = null!;

    public string StudentId { get; set; } = string.Empty;
    public ApplicationUser Student { get; set; } = null!;

    /// <summary>Free-text content entered by student (optional if file is submitted).</summary>
    public string? TextContent { get; set; }

    public SubmissionStatus Status { get; set; } = SubmissionStatus.OnTime;

    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    // --- Navigation ---
    public ICollection<SubmissionFile> Files { get; set; } = [];
    public Grade? Grade { get; set; }
    public AIAnalysisResult? AIAnalysis { get; set; }
}

/// <summary>
/// File(s) uploaded by Student as part of a Submission.
/// Stored on local disk — path saved in DB.
/// </summary>
public class SubmissionFile : BaseEntity
{
    public int Id { get; set; }

    public int SubmissionId { get; set; }
    public Submission Submission { get; set; } = null!;

    public string FileName { get; set; } = string.Empty;

    /// <summary>Relative path — e.g. /uploads/submissions/42/essay.docx</summary>
    public string FilePath { get; set; } = string.Empty;

    public long FileSizeBytes { get; set; }
    public string ContentType { get; set; } = string.Empty;
}

/// <summary>
/// FR-10: Grade given by Teacher after reviewing a Submission.
/// Triggers SignalR notification to Student and Parent.
/// </summary>
public class Grade : BaseEntity
{
    public int Id { get; set; }

    public int SubmissionId { get; set; }
    public Submission Submission { get; set; } = null!;

    public string GradedByTeacherId { get; set; } = string.Empty;
    public ApplicationUser GradedByTeacher { get; set; } = null!;

    /// <summary>Score entered by Teacher (0–10 or custom scale).</summary>
    public decimal Score { get; set; }

    public string? Feedback { get; set; }

    public DateTime GradedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// FR-07: Result from the AI Analysis Service after a Submission is received.
/// Stored for Teacher to review before grading.
/// </summary>
public class AIAnalysisResult : BaseEntity
{
    public int Id { get; set; }

    public int SubmissionId { get; set; }
    public Submission Submission { get; set; } = null!;

    /// <summary>Percentage of content estimated to be AI-generated (0–100).</summary>
    public decimal AIUsagePercent { get; set; }

    /// <summary>Score suggested by AI based on content quality.</summary>
    public decimal? SuggestedScore { get; set; }

    /// <summary>Natural-language summary of the analysis.</summary>
    public string? AnalysisSummary { get; set; }

    public DateTime AnalyzedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Track if AI analysis is still pending (async job not yet complete).
    /// </summary>
    public bool IsCompleted { get; set; } = false;
}

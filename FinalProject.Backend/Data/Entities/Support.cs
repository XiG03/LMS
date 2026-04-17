using System;

namespace FinalProject.Backend.Data.Entities;

/// <summary>
/// FR-19: OTP code for Forgot Password flow.
/// Sent via SMTP. Cleaned up by Hangfire job after expiry.
/// </summary>
public class OtpCode
{
    public int Id { get; set; }

    public string Email { get; set; } = string.Empty;

    /// <summary>6-digit code sent to user.</summary>
    public string Code { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// FR-18: Audit log entry — every significant system action is recorded.
/// Admin can query this for traceability.
/// </summary>
public class AuditLog
{
    public long Id { get; set; }

    /// <summary>User who performed the action. Null for system actions.</summary>
    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }

    /// <summary>e.g. "Assignment.Create", "Grade.Update", "User.Approve"</summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>Entity type affected — e.g. "Assignment"</summary>
    public string? EntityName { get; set; }

    /// <summary>PK of the affected entity as string.</summary>
    public string? EntityId { get; set; }

    /// <summary>JSON snapshot of old values (for updates/deletes).</summary>
    public string? OldValues { get; set; }

    /// <summary>JSON snapshot of new values (for creates/updates).</summary>
    public string? NewValues { get; set; }

    public string? IpAddress { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Persisted notification record — mirrors SignalR real-time events.
/// Allows users to see missed notifications when they reconnect.
/// </summary>
public class Notification : BaseEntity
{
    public int Id { get; set; }

    /// <summary>Recipient of this notification.</summary>
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    /// <summary>
    /// Maps to SignalR event names:
    /// UserApproved | UserAddedToClass | AccountLinked |
    /// NewAssignment | NewSubmission | AssignmentGraded |
    /// GradePublished | DeadlineReminder
    /// </summary>
    public string EventType { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;
    public string? Body { get; set; }

    /// <summary>Optional deep-link data — e.g. { "assignmentId": 5 }</summary>
    public string? Payload { get; set; }

    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }
}

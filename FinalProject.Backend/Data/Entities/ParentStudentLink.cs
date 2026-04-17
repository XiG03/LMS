using System;

namespace FinalProject.Backend.Data.Entities;

public enum LinkStatus
{
    Pending,   // Parent submitted request, waiting for Admin approval
    Approved,  // Admin confirmed — Parent can now view Student's grades
    Rejected   // Admin rejected the request
}

/// <summary>
/// FR-16: Links a Parent account to a Student account.
/// Flow: Parent enters StudentLinkCode → sends request → Admin approves.
/// </summary>
public class ParentStudentLink : BaseEntity
{
    public int Id { get; set; }

    public string ParentId { get; set; } = string.Empty;
    public ApplicationUser Parent { get; set; } = null!;

    public string StudentId { get; set; } = string.Empty;
    public ApplicationUser Student { get; set; } = null!;

    public LinkStatus Status { get; set; } = LinkStatus.Pending;

    /// <summary>Set by Admin when approving or rejecting.</summary>
    public string? ReviewedByAdminId { get; set; }
    public DateTime? ReviewedAt { get; set; }

    public string? RejectionReason { get; set; }
}

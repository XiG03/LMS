using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace FinalProject.Backend.Data.Entities;

/// <summary>
/// Extended Identity user — shared by Teacher, Student, Parent, Admin.
/// Role is managed via ASP.NET Identity Roles (not an enum column).
/// After registration, user gets role "Unknown" until Admin approves.
/// </summary>
public class ApplicationUser : IdentityUser
{
    // --- Profile ---
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Personal phone number — used for Parent-Student linking.
    /// </summary>
    public string PersonalPhone { get; set; } = string.Empty;

    /// <summary>
    /// Unique code per Student, used by Parent to initiate linking.
    /// Only meaningful when user is a Student.
    /// </summary>
    public string? StudentLinkCode { get; set; }

    /// <summary>
    /// Phone of parent — filled in by Student at registration for Admin cross-check.
    /// </summary>
    public string? ParentPhone { get; set; }

    // --- Account state ---
    public bool IsActive { get; set; } = true;

    // --- JWT Refresh Token ---
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }

    // --- Soft Delete & Audit ---
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    // --- Navigation ---
    public ICollection<ClassEnrollment> ClassEnrollments { get; set; } = [];
    public ICollection<Assignment> Assignments { get; set; } = [];          // Teacher creates
    public ICollection<Submission> Submissions { get; set; } = [];          // Student submits
    public ICollection<ParentStudentLink> ParentLinks { get; set; } = [];   // Parent side
    public ICollection<ParentStudentLink> StudentLinks { get; set; } = [];  // Student side
    public ICollection<Notification> Notifications { get; set; } = [];
    public ICollection<AuditLog> AuditLogs { get; set; } = [];
}

using System;
using System.Collections.Generic;

namespace FinalProject.Backend.Data.Entities;

/// <summary>
/// FR-13: Admin-managed subject catalogue (Môn học).
/// </summary>
public class Subject : BaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    // --- Navigation ---
    public ICollection<Assignment> Assignments { get; set; } = [];
    public ICollection<ClassSubject> ClassSubjects { get; set; } = [];
}

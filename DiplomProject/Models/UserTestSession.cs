using System;
using System.Collections.Generic;

namespace DiplomProject.Models;

public partial class UserTestSession
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int TestId { get; set; }

    public int? CurrentQuestion { get; set; }

    public string? SelectedAnswers { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsCompleted { get; set; }

    public virtual Test Test { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}

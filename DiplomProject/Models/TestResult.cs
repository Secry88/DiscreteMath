using System;
using System.Collections.Generic;

namespace DiscreteMath.Models;

public partial class TestResult
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int TestId { get; set; }

    public int Score { get; set; }

    public int TotalQuestions { get; set; }

    public DateTime? CompletedAt { get; set; }

    public int? AttemptNumber { get; set; }

    public int? Percentage { get; set; }

    public virtual Test Test { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}

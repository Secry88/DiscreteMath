using System;
using System.Collections.Generic;

namespace DiplomProject.Models;

public partial class Test
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();

    public virtual ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();
}

using System;
using System.Collections.Generic;

namespace DiscreteMath.Models;

public partial class EulerRegion
{
    public int Id { get; set; }

    public int ProblemId { get; set; }

    public string RegionCode { get; set; } = null!;

    public bool IsCorrect { get; set; }

    public virtual EulerProblem Problem { get; set; } = null!;
}

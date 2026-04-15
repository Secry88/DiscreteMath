using System;
using System.Collections.Generic;

namespace DiplomProject.Models;

public partial class EulerProblem
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string Description { get; set; } = null!;

    public int DiagramType { get; set; }

    public int? Difficulty { get; set; }

    public virtual ICollection<EulerRegion> EulerRegions { get; set; } = new List<EulerRegion>();
}

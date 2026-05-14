using System;
using System.Collections.Generic;

namespace DiplomProject.Models;

public partial class SetOperationTask
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string Expression { get; set; } = null!;

    public string? SetA { get; set; }

    public string? SetB { get; set; }

    public string? SetC { get; set; }

    public string? UniversalSet { get; set; }

    public int DiagramType { get; set; }

    public int Difficulty { get; set; }

    public virtual ICollection<SetOperationCorrectRegion> SetOperationCorrectRegions { get; set; } = new List<SetOperationCorrectRegion>();

    public virtual ICollection<SetOperationStep> SetOperationSteps { get; set; } = new List<SetOperationStep>();
}

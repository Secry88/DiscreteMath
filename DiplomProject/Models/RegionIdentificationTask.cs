using System;
using System.Collections.Generic;

namespace DiscreteMath.Models;

public partial class RegionIdentificationTask
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? SetA { get; set; }

    public string? SetB { get; set; }

    public string? SetC { get; set; }

    public string? UniversalSet { get; set; }

    public int DiagramType { get; set; }

    public int Difficulty { get; set; }

    public string? DiagramImage { get; set; }

    public virtual ICollection<RegionIdentificationElement> RegionIdentificationElements { get; set; } = new List<RegionIdentificationElement>();
}

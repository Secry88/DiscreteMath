using System;
using System.Collections.Generic;

namespace DiplomProject.Models;

public partial class RegionIdentificationElement
{
    public int Id { get; set; }

    public int TaskId { get; set; }

    public int ElementValue { get; set; }

    public int CorrectRegionNumber { get; set; }

    public virtual RegionIdentificationTask Task { get; set; } = null!;
}

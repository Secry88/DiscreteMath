using System;
using System.Collections.Generic;

namespace DiscreteMath.Models;

public partial class SetOperationCorrectRegion
{
    public int Id { get; set; }

    public int TaskId { get; set; }

    public string RegionCode { get; set; } = null!;

    public virtual SetOperationTask Task { get; set; } = null!;
}

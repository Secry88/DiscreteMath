using System;
using System.Collections.Generic;

namespace DiscreteMath.Models;

public partial class TheoryCategory
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<TheoryTopic> TheoryTopics { get; set; } = new List<TheoryTopic>();
}

using System;
using System.Collections.Generic;

namespace DiscreteMath.Models;

public partial class TheoryTopic
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public string Title { get; set; } = null!;

    public int? OrderIndex { get; set; }

    public virtual TheoryCategory Category { get; set; } = null!;

    public virtual ICollection<TheoryContent> TheoryContents { get; set; } = new List<TheoryContent>();

    public virtual ICollection<UserTheoryProgress> UserTheoryProgresses { get; set; } = new List<UserTheoryProgress>();
}

using System;
using System.Collections.Generic;

namespace DiscreteMath.Models;

public partial class UserTheoryProgress
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int TopicId { get; set; }

    public DateTime? CompletedAt { get; set; }

    public virtual TheoryTopic Topic { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}

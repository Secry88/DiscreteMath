using System;
using System.Collections.Generic;

namespace DiplomProject.Models;

public partial class TheoryContent
{
    public int Id { get; set; }

    public int TopicId { get; set; }

    public string Content { get; set; } = null!;

    public int? OrderIndex { get; set; }

    public virtual TheoryTopic Topic { get; set; } = null!;
}

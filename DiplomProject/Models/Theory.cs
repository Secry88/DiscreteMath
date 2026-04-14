using System;
using System.Collections.Generic;

namespace DiplomProject.Models;

public partial class Theory
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;
}

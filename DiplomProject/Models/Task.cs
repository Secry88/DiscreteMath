using System;
using System.Collections.Generic;

namespace DiscreteMath.Models;

public partial class Task
{
    public int Id { get; set; }

    public string Type { get; set; } = null!;

    public int Subtype { get; set; }

    public string Condition { get; set; } = null!;

    public string CorrectAnswer { get; set; } = null!;

    public string? SetA { get; set; }

    public string? SetB { get; set; }

    public string? Operation { get; set; }
}

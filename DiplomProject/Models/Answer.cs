using System;
using System.Collections.Generic;

namespace DiscreteMath.Models;

public partial class Answer
{
    public int Id { get; set; }

    public int QuestionId { get; set; }

    public string AnswerText { get; set; } = null!;

    public bool IsCorrect { get; set; }

    public int? OrderIndex { get; set; }

    public string? Explanation { get; set; }

    public virtual Question Question { get; set; } = null!;
}

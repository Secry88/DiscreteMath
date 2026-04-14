using System;
using System.Collections.Generic;

namespace DiplomProject.Models;

public partial class Question
{
    public int Id { get; set; }

    public int TestId { get; set; }

    public string QuestionText { get; set; } = null!;

    public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();

    public virtual Test Test { get; set; } = null!;
}

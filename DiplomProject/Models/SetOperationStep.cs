using System;
using System.Collections.Generic;

namespace DiplomProject.Models;

public partial class SetOperationStep
{
    public int Id { get; set; }

    public int TaskId { get; set; }

    public int StepNumber { get; set; }

    public string Description { get; set; } = null!;

    public string Expression { get; set; } = null!;

    public string CorrectAnswer { get; set; } = null!;

    public virtual SetOperationTask Task { get; set; } = null!;
}

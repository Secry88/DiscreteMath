namespace DiplomProject.Models;

public class SetOperationStepDto
{
    public int Id { get; set; }
    public int StepNumber { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Expression { get; set; } = string.Empty;
    public string CorrectAnswer { get; set; } = string.Empty;
}

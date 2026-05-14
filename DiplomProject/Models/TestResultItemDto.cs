namespace DiscreteMath.Models;

public class TestResultItemDto
{
    public string TestTitle { get; set; } = string.Empty;
    public int Score { get; set; }
    public int TotalQuestions { get; set; }
    public int Percentage { get; set; }
    public string DateText { get; set; } = string.Empty;
    public string ScoreText => $"{Score} / {TotalQuestions}";
    public string PercentageText => $"{Percentage}%";
    public bool IsPassed => Percentage >= 60;
}

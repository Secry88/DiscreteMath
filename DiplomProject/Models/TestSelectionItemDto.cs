namespace DiplomProject.Models
{
    public class TestSelectionItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
        public int QuestionsCount { get; set; }
        public int DurationMinutes { get; set; }
        public int Difficulty { get; set; }
        public string DifficultyLabel { get; set; } = "Medium";
        public bool IsCompleted { get; set; }
        public int? LastPercentage { get; set; }

        public bool IsEasy => Difficulty == 1;
        public bool IsMedium => Difficulty == 2;
        public bool IsHard => Difficulty >= 3;
        public string StartButtonText => IsCompleted ? "Retake Test" : "Start Test";
    }
}

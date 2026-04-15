namespace DiplomProject.Models
{
    public class PracticeTaskDto
    {
        public int Id { get; set; }
        public string SetA { get; set; } = string.Empty;
        public string SetB { get; set; } = string.Empty;
        public string Operation { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty;
        public string CorrectAnswer { get; set; } = string.Empty;
        public int Subtype { get; set; }
    }
}

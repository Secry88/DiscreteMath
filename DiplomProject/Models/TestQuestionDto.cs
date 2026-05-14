using System.Collections.Generic;

namespace DiscreteMath.Models
{
    public class TestQuestionDto
    {
        public int Id { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public int OrderIndex { get; set; }
        public List<TestAnswerDto> Answers { get; set; } = new();
    }
}

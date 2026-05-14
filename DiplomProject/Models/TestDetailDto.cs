using System.Collections.Generic;

namespace DiscreteMath.Models
{
    public class TestDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<TestQuestionDto> Questions { get; set; } = new();
    }
}

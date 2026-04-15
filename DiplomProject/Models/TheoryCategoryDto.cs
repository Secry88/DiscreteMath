using System.Collections.Generic;

namespace DiplomProject.Models
{
    public class TheoryCategoryDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<TheoryTopicDto> Topics { get; set; } = new();
    }
}

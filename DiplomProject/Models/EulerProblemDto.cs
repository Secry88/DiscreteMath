using System.Collections.Generic;

namespace DiplomProject.Models
{
    public class EulerProblemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DiagramType { get; set; }
        public int Difficulty { get; set; } = 1;
        public List<EulerRegionDto> Regions { get; set; } = new();
    }
}

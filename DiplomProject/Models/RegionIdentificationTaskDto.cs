using System.Collections.Generic;

namespace DiscreteMath.Models;

public class RegionIdentificationTaskDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string SetA { get; set; } = string.Empty;
    public string SetB { get; set; } = string.Empty;
    public string SetC { get; set; } = string.Empty;
    public string UniversalSet { get; set; } = string.Empty;
    public int DiagramType { get; set; } = 2;
    public int Difficulty { get; set; } = 1;
    public string? DiagramImageBase64 { get; set; }
    public List<RegionIdentificationElementDto> Elements { get; set; } = new();
}

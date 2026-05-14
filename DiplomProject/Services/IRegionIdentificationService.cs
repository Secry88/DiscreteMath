using DiscreteMath.Models;
using System.Collections.Generic;

namespace DiscreteMath.Services;

public interface IRegionIdentificationService
{
    List<RegionIdentificationTaskDto> GetTasks();
    void AddTask(RegionIdentificationTaskDto dto);
    void UpdateTask(RegionIdentificationTaskDto dto);
    void DeleteTask(int id);
}

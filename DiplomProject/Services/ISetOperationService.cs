using DiscreteMath.Models;
using System.Collections.Generic;

namespace DiscreteMath.Services;

public interface ISetOperationService
{
    List<SetOperationTaskDto> GetTasks();
    void AddTask(SetOperationTaskDto dto);
    void UpdateTask(SetOperationTaskDto dto);
    void DeleteTask(int id);
}

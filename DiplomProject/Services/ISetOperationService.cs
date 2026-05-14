using DiplomProject.Models;
using System.Collections.Generic;

namespace DiplomProject.Services;

public interface ISetOperationService
{
    List<SetOperationTaskDto> GetTasks();
    void AddTask(SetOperationTaskDto dto);
    void UpdateTask(SetOperationTaskDto dto);
    void DeleteTask(int id);
}

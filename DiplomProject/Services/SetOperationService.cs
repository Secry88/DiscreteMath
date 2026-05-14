using DiscreteMath.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace DiscreteMath.Services;

public class SetOperationService : ISetOperationService
{
    private readonly KarmanovContext _db;

    public SetOperationService(KarmanovContext db) => _db = db;

    public List<SetOperationTaskDto> GetTasks()
    {
        return _db.SetOperationTasks
            .Include(x => x.SetOperationSteps)
            .Include(x => x.SetOperationCorrectRegions)
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Select(t => new SetOperationTaskDto
            {
                Id = t.Id,
                Title = t.Title ?? string.Empty,
                Description = t.Description ?? string.Empty,
                Expression = t.Expression,
                SetA = t.SetA ?? string.Empty,
                SetB = t.SetB ?? string.Empty,
                SetC = t.SetC ?? string.Empty,
                UniversalSet = t.UniversalSet ?? string.Empty,
                DiagramType = t.DiagramType,
                Difficulty = t.Difficulty,
                Steps = t.SetOperationSteps
                    .OrderBy(s => s.StepNumber)
                    .Select(s => new SetOperationStepDto
                    {
                        Id = s.Id,
                        StepNumber = s.StepNumber,
                        Description = s.Description,
                        Expression = s.Expression,
                        CorrectAnswer = s.CorrectAnswer
                    }).ToList(),
                CorrectRegions = t.SetOperationCorrectRegions
                    .Select(r => r.RegionCode).ToList()
            })
            .ToList();
    }

    public void AddTask(SetOperationTaskDto dto)
    {
        var task = new SetOperationTask
        {
            Title = dto.Title,
            Description = dto.Description,
            Expression = dto.Expression,
            SetA = dto.SetA,
            SetB = dto.SetB,
            SetC = dto.SetC,
            UniversalSet = dto.UniversalSet,
            DiagramType = dto.DiagramType,
            Difficulty = dto.Difficulty
        };
        _db.SetOperationTasks.Add(task);
        _db.SaveChanges();
        SaveStepsAndRegions(task.Id, dto);
    }

    public void UpdateTask(SetOperationTaskDto dto)
    {
        var task = _db.SetOperationTasks.Find(dto.Id);
        if (task is null) return;

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.Expression = dto.Expression;
        task.SetA = dto.SetA;
        task.SetB = dto.SetB;
        task.SetC = dto.SetC;
        task.UniversalSet = dto.UniversalSet;
        task.DiagramType = dto.DiagramType;
        task.Difficulty = dto.Difficulty;

        _db.SetOperationSteps.RemoveRange(
            _db.SetOperationSteps.Where(s => s.TaskId == dto.Id));
        _db.SetOperationCorrectRegions.RemoveRange(
            _db.SetOperationCorrectRegions.Where(r => r.TaskId == dto.Id));
        _db.SaveChanges();

        SaveStepsAndRegions(dto.Id, dto);
    }

    public void DeleteTask(int id)
    {
        var task = _db.SetOperationTasks.Find(id);
        if (task is null) return;
        _db.SetOperationTasks.Remove(task);
        _db.SaveChanges();
    }

    private void SaveStepsAndRegions(int taskId, SetOperationTaskDto dto)
    {
        for (int i = 0; i < dto.Steps.Count; i++)
        {
            var s = dto.Steps[i];
            _db.SetOperationSteps.Add(new SetOperationStep
            {
                TaskId = taskId,
                StepNumber = i,
                Description = s.Description,
                Expression = s.Expression,
                CorrectAnswer = s.CorrectAnswer
            });
        }
        foreach (var code in dto.CorrectRegions)
        {
            _db.SetOperationCorrectRegions.Add(new SetOperationCorrectRegion
            {
                TaskId = taskId,
                RegionCode = code
            });
        }
        _db.SaveChanges();
    }
}

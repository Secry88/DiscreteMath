using DiplomProject.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace DiplomProject.Services;

public class RegionIdentificationService : IRegionIdentificationService
{
    private readonly KarmanovContext _db;

    public RegionIdentificationService(KarmanovContext db) => _db = db;

    public List<RegionIdentificationTaskDto> GetTasks()
    {
        return _db.RegionIdentificationTasks
            .Include(x => x.RegionIdentificationElements)
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Select(t => new RegionIdentificationTaskDto
            {
                Id = t.Id,
                Title = t.Title ?? string.Empty,
                Description = t.Description ?? string.Empty,
                SetA = t.SetA ?? string.Empty,
                SetB = t.SetB ?? string.Empty,
                SetC = t.SetC ?? string.Empty,
                UniversalSet = t.UniversalSet ?? string.Empty,
                DiagramType = t.DiagramType,
                Difficulty = t.Difficulty,
                DiagramImageBase64 = t.DiagramImage,
                Elements = t.RegionIdentificationElements
                    .OrderBy(e => e.ElementValue)
                    .Select(e => new RegionIdentificationElementDto
                    {
                        Id = e.Id,
                        ElementValue = e.ElementValue,
                        CorrectRegionNumber = e.CorrectRegionNumber
                    }).ToList()
            })
            .ToList();
    }

    public void AddTask(RegionIdentificationTaskDto dto)
    {
        var task = new RegionIdentificationTask
        {
            Title = dto.Title,
            Description = dto.Description,
            SetA = dto.SetA,
            SetB = dto.SetB,
            SetC = dto.SetC,
            UniversalSet = dto.UniversalSet,
            DiagramType = dto.DiagramType,
            Difficulty = dto.Difficulty,
            DiagramImage = dto.DiagramImageBase64
        };
        _db.RegionIdentificationTasks.Add(task);
        _db.SaveChanges();
        SaveElements(task.Id, dto);
    }

    public void UpdateTask(RegionIdentificationTaskDto dto)
    {
        var task = _db.RegionIdentificationTasks.Find(dto.Id);
        if (task is null) return;

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.SetA = dto.SetA;
        task.SetB = dto.SetB;
        task.SetC = dto.SetC;
        task.UniversalSet = dto.UniversalSet;
        task.DiagramType = dto.DiagramType;
        task.Difficulty = dto.Difficulty;
        if (dto.DiagramImageBase64 is not null)
            task.DiagramImage = dto.DiagramImageBase64;

        _db.RegionIdentificationElements.RemoveRange(
            _db.RegionIdentificationElements.Where(e => e.TaskId == dto.Id));
        _db.SaveChanges();

        SaveElements(dto.Id, dto);
    }

    public void DeleteTask(int id)
    {
        var task = _db.RegionIdentificationTasks.Find(id);
        if (task is null) return;
        _db.RegionIdentificationTasks.Remove(task);
        _db.SaveChanges();
    }

    private void SaveElements(int taskId, RegionIdentificationTaskDto dto)
    {
        foreach (var elem in dto.Elements)
        {
            _db.RegionIdentificationElements.Add(new RegionIdentificationElement
            {
                TaskId = taskId,
                ElementValue = elem.ElementValue,
                CorrectRegionNumber = elem.CorrectRegionNumber
            });
        }
        _db.SaveChanges();
    }
}

using DiscreteMath.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SysTask = System.Threading.Tasks.Task;

namespace DiscreteMath.Services
{
    public class PracticeService : IPracticeService
    {
        private readonly KarmanovContext _db;

        public PracticeService(KarmanovContext db)
        {
            _db = db;
        }

        public async Task<List<PracticeTaskDto>> GetPracticeTasksAsync()
        {
            return await _db.Tasks
                .AsNoTracking()
                .Where(x => x.Type == "practice")
                .OrderBy(x => x.Id)
                .Select(x => new PracticeTaskDto
                {
                    Id = x.Id,
                    SetA = x.SetA ?? string.Empty,
                    SetB = x.SetB ?? string.Empty,
                    Operation = x.Operation ?? string.Empty,
                    Condition = x.Condition,
                    CorrectAnswer = x.CorrectAnswer,
                    Subtype = x.Subtype
                })
                .ToListAsync();
        }

        // ── CRUD ─────────────────────────────────────────────────────────────────

        public async SysTask AddTaskAsync(string setA, string setB, string operation,
            string condition, string correctAnswer, int subtype)
        {
            _db.Tasks.Add(new Models.Task
            {
                Type = "practice",
                SetA = setA,
                SetB = setB,
                Operation = operation,
                Condition = condition,
                CorrectAnswer = correctAnswer,
                Subtype = subtype
            });
            await _db.SaveChangesAsync();
        }

        public async SysTask UpdateTaskAsync(int id, string setA, string setB, string operation,
            string condition, string correctAnswer)
        {
            var task = await _db.Tasks.FindAsync(id);
            if (task is null) return;
            task.SetA = setA;
            task.SetB = setB;
            task.Operation = operation;
            task.Condition = condition;
            task.CorrectAnswer = correctAnswer;
            await _db.SaveChangesAsync();
        }

        public async SysTask DeleteTaskAsync(int id)
        {
            var task = await _db.Tasks.FindAsync(id);
            if (task is null) return;
            _db.Tasks.Remove(task);
            await _db.SaveChangesAsync();
        }
    }
}

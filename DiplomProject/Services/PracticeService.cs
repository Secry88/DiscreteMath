using DiplomProject.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiplomProject.Services
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
    }
}

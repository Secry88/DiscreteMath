using DiscreteMath.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace DiscreteMath.Services
{
    public class EulerService
    {
        private readonly KarmanovContext _db;

        public EulerService(KarmanovContext db)
        {
            _db = db;
        }

        public List<EulerProblemDto> GetProblems()
        {
            return _db.EulerProblems
                .Include(x => x.EulerRegions)
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(problem => new EulerProblemDto
                {
                    Id = problem.Id,
                    Title = problem.Title ?? string.Empty,
                    Description = problem.Description,
                    DiagramType = problem.DiagramType,
                    Difficulty = problem.Difficulty ?? 1,
                    Regions = problem.EulerRegions
                        .Select(region => new EulerRegionDto
                        {
                            RegionCode = region.RegionCode,
                            IsCorrect = region.IsCorrect
                        })
                        .ToList()
                })
                .ToList();
        }

        // ── CRUD ─────────────────────────────────────────────────────────────────

        public void AddProblem(string title, string description, int diagramType, int difficulty,
            bool aOnlyCorrect, bool abCorrect, bool bOnlyCorrect)
        {
            var problem = new EulerProblem
            {
                Title = title,
                Description = description,
                DiagramType = diagramType,
                Difficulty = difficulty
            };
            _db.EulerProblems.Add(problem);
            _db.SaveChanges();

            AddRegions(problem.Id, aOnlyCorrect, abCorrect, bOnlyCorrect);
        }

        public void UpdateProblem(int id, string title, string description, int difficulty,
            bool aOnlyCorrect, bool abCorrect, bool bOnlyCorrect)
        {
            var problem = _db.EulerProblems.Find(id);
            if (problem is null) return;
            problem.Title = title;
            problem.Description = description;
            problem.Difficulty = difficulty;

            var existing = _db.EulerRegions.Where(r => r.ProblemId == id).ToList();
            _db.EulerRegions.RemoveRange(existing);
            _db.SaveChanges();

            AddRegions(id, aOnlyCorrect, abCorrect, bOnlyCorrect);
        }

        public void DeleteProblem(int id)
        {
            var problem = _db.EulerProblems.Find(id);
            if (problem is null) return;
            _db.EulerProblems.Remove(problem);
            _db.SaveChanges();
        }

        private void AddRegions(int problemId, bool aOnly, bool ab, bool bOnly)
        {
            _db.EulerRegions.AddRange(
                new EulerRegion { ProblemId = problemId, RegionCode = "a-only", IsCorrect = aOnly },
                new EulerRegion { ProblemId = problemId, RegionCode = "ab",     IsCorrect = ab    },
                new EulerRegion { ProblemId = problemId, RegionCode = "b-only", IsCorrect = bOnly }
            );
            _db.SaveChanges();
        }
    }
}

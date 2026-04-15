using DiplomProject.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace DiplomProject.Services
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
                    Description = problem.Description,
                    DiagramType = problem.DiagramType,
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
    }
}

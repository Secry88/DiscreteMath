using DiplomProject.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace DiplomProject.Services
{
    public class TheoryService
    {
        private readonly KarmanovContext _db;

        public TheoryService(KarmanovContext db)
        {
            _db = db;
        }

        public TheoryCategoryDto? GetCategory(int categoryId)
        {
            var category = _db.TheoryCategories
                .Include(x => x.TheoryTopics)
                .ThenInclude(x => x.TheoryContents)
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == categoryId);

            if (category is null)
            {
                return null;
            }

            return new TheoryCategoryDto
            {
                Id = category.Id,
                Title = category.Title,
                Description = category.Description ?? string.Empty,
                Topics = category.TheoryTopics
                    .OrderBy(x => x.OrderIndex ?? int.MaxValue)
                    .ThenBy(x => x.Id)
                    .Select(topic => new TheoryTopicDto
                    {
                        Id = topic.Id,
                        Title = topic.Title,
                        OrderIndex = topic.OrderIndex ?? 0,
                        Contents = topic.TheoryContents
                            .OrderBy(x => x.OrderIndex ?? int.MaxValue)
                            .ThenBy(x => x.Id)
                            .Select(content => new TheoryContentDto
                            {
                                Id = content.Id,
                                Content = content.Content,
                                OrderIndex = content.OrderIndex ?? 0
                            })
                            .ToList()
                    })
                    .ToList()
            };
        }

        public List<TheoryCategoryDto> GetCategories()
        {
            return _db.TheoryCategories
                .AsNoTracking()
                .Select(x => new TheoryCategoryDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description ?? string.Empty
                })
                .ToList();
        }
    }
}

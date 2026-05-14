using DiscreteMath.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace DiscreteMath.Services
{
    public class TheoryService
    {
        private readonly KarmanovContext _db;

        public TheoryService(KarmanovContext db)
        {
            _db = db;
        }

        // ── Category CRUD ────────────────────────────────────────────────────────

        public void AddCategory(string title, string description)
        {
            _db.TheoryCategories.Add(new TheoryCategory { Title = title, Description = description });
            _db.SaveChanges();
        }

        public void UpdateCategory(int id, string title, string description)
        {
            var cat = _db.TheoryCategories.Find(id);
            if (cat is null) return;
            cat.Title = title;
            cat.Description = description;
            _db.SaveChanges();
        }

        public void DeleteCategory(int id)
        {
            var cat = _db.TheoryCategories.Find(id);
            if (cat is null) return;
            _db.TheoryCategories.Remove(cat);
            _db.SaveChanges();
        }

        // ── Topic CRUD ───────────────────────────────────────────────────────────

        public void AddTopic(int categoryId, string title)
        {
            _db.TheoryTopics.Add(new TheoryTopic { CategoryId = categoryId, Title = title });
            _db.SaveChanges();
        }

        public void UpdateTopic(int id, string title)
        {
            var topic = _db.TheoryTopics.Find(id);
            if (topic is null) return;
            topic.Title = title;
            _db.SaveChanges();
        }

        public void DeleteTopic(int id)
        {
            var topic = _db.TheoryTopics.Find(id);
            if (topic is null) return;
            _db.TheoryTopics.Remove(topic);
            _db.SaveChanges();
        }

        // ── Content CRUD ─────────────────────────────────────────────────────────

        public List<TheoryContentDto> GetContentsForTopic(int topicId)
        {
            return _db.TheoryContents
                .AsNoTracking()
                .Where(x => x.TopicId == topicId)
                .OrderBy(x => x.OrderIndex ?? int.MaxValue)
                .ThenBy(x => x.Id)
                .Select(x => new TheoryContentDto
                {
                    Id = x.Id,
                    Content = x.Content,
                    OrderIndex = x.OrderIndex ?? 0
                })
                .ToList();
        }

        public void AddContent(int topicId, string content)
        {
            _db.TheoryContents.Add(new TheoryContent { TopicId = topicId, Content = content });
            _db.SaveChanges();
        }

        public void UpdateContent(int id, string content)
        {
            var c = _db.TheoryContents.Find(id);
            if (c is null) return;
            c.Content = content;
            _db.SaveChanges();
        }

        public void DeleteContent(int id)
        {
            var c = _db.TheoryContents.Find(id);
            if (c is null) return;
            _db.TheoryContents.Remove(c);
            _db.SaveChanges();
        }

        // ── Read ─────────────────────────────────────────────────────────────────

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

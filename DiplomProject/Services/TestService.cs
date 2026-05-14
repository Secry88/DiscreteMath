using DiscreteMath.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace DiscreteMath.Services
{
    public class TestService : ITestService
    {
        private readonly KarmanovContext _db;

        public TestService(KarmanovContext db)
        {
            _db = db;
        }

        public async Task<List<TestSelectionItemDto>> GetTestsForSelectionAsync(int userId)
        {
            var tests = await _db.Tests
                .Include(x => x.Questions)
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .ToListAsync();

            var latestResults = await _db.TestResults
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .GroupBy(x => x.TestId)
                .Select(g => g.OrderByDescending(x => x.CompletedAt).ThenByDescending(x => x.Id).First())
                .ToListAsync();

            var resultsMap = latestResults.ToDictionary(x => x.TestId, x => x);

            return tests.Select(test =>
            {
                resultsMap.TryGetValue(test.Id, out var result);

                return new TestSelectionItemDto
                {
                    Id = test.Id,
                    Title = test.Title,
                    Description = test.Description ?? string.Empty,
                    Topic = test.Topic ?? "Operations on Sets",
                    QuestionsCount = test.Questions.Count,
                    DurationMinutes = test.Duration ?? 15,
                    Difficulty = test.Difficulty ?? 2,
                    DifficultyLabel = MapDifficultyLabel(test.Difficulty ?? 2),
                    IsCompleted = result is not null,
                    LastPercentage = result?.Percentage
                };
            }).ToList();
        }

        public async Task<TestDetailDto?> GetTestDetailAsync(int testId)
        {
            var test = await _db.Tests
                .Include(x => x.Questions.OrderBy(q => q.OrderIndex ?? int.MaxValue).ThenBy(q => q.Id))
                .ThenInclude(x => x.Answers.OrderBy(a => a.OrderIndex ?? int.MaxValue).ThenBy(a => a.Id))
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == testId);

            if (test is null)
            {
                return null;
            }

            return new TestDetailDto
            {
                Id = test.Id,
                Title = test.Title,
                Description = test.Description ?? string.Empty,
                Questions = test.Questions
                    .OrderBy(q => q.OrderIndex ?? int.MaxValue)
                    .ThenBy(q => q.Id)
                    .Select(question => new TestQuestionDto
                    {
                        Id = question.Id,
                        QuestionText = question.QuestionText,
                        OrderIndex = question.OrderIndex ?? 0,
                        Answers = question.Answers
                            .OrderBy(answer => answer.OrderIndex ?? int.MaxValue)
                            .ThenBy(answer => answer.Id)
                            .Select(answer => new TestAnswerDto
                            {
                                Id = answer.Id,
                                AnswerText = answer.AnswerText,
                                IsCorrect = answer.IsCorrect
                            })
                            .ToList()
                    })
                    .ToList()
            };
        }

        public async Task SaveTestResultAsync(int userId, int testId, int score, int totalQuestions)
        {
            var attemptNumber = (await _db.TestResults
                .AsNoTracking()
                .Where(x => x.UserId == userId && x.TestId == testId)
                .CountAsync()) + 1;

            var percentage = totalQuestions == 0
                ? 0
                : (int)Math.Round((double)score / totalQuestions * 100.0, MidpointRounding.AwayFromZero);

            var result = new TestResult
            {
                UserId = userId,
                TestId = testId,
                Score = score,
                TotalQuestions = totalQuestions,
                CompletedAt = DateTime.Now,
                AttemptNumber = attemptNumber,
                Percentage = percentage
            };

            _db.TestResults.Add(result);
            await _db.SaveChangesAsync();
        }

        private static string MapDifficultyLabel(int difficulty)
        {
            return difficulty switch
            {
                <= 1 => "Easy",
                2 => "Medium",
                _ => "Hard"
            };
        }

        // ── Tests CRUD ───────────────────────────────────────────────────────────

        public async Task<List<TestSelectionItemDto>> GetAllTestsForTeacherAsync()
        {
            return await _db.Tests
                .Include(x => x.Questions)
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(t => new TestSelectionItemDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description ?? string.Empty,
                    Topic = t.Topic ?? string.Empty,
                    QuestionsCount = t.Questions.Count,
                    DurationMinutes = t.Duration ?? 15,
                    Difficulty = t.Difficulty ?? 1,
                    DifficultyLabel = MapDifficultyLabel(t.Difficulty ?? 1),
                    IsCompleted = false
                })
                .ToListAsync();
        }

        public async Task AddTestAsync(string title, string description, string topic, int difficulty, int duration)
        {
            _db.Tests.Add(new Test
            {
                Title = title,
                Description = description,
                Topic = topic,
                Difficulty = difficulty,
                Duration = duration
            });
            await _db.SaveChangesAsync();
        }

        public async Task UpdateTestAsync(int id, string title, string description, string topic, int difficulty, int duration)
        {
            var test = await _db.Tests.FindAsync(id);
            if (test is null) return;
            test.Title = title;
            test.Description = description;
            test.Topic = topic;
            test.Difficulty = difficulty;
            test.Duration = duration;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteTestAsync(int id)
        {
            var test = await _db.Tests.FindAsync(id);
            if (test is null) return;
            _db.Tests.Remove(test);
            await _db.SaveChangesAsync();
        }

        // ── Questions CRUD ───────────────────────────────────────────────────────

        public async Task<List<TestQuestionDto>> GetQuestionsAsync(int testId)
        {
            return await _db.Questions
                .Include(q => q.Answers)
                .AsNoTracking()
                .Where(q => q.TestId == testId)
                .OrderBy(q => q.OrderIndex ?? int.MaxValue).ThenBy(q => q.Id)
                .Select(q => new TestQuestionDto
                {
                    Id = q.Id,
                    QuestionText = q.QuestionText,
                    OrderIndex = q.OrderIndex ?? 0,
                    Answers = q.Answers
                        .OrderBy(a => a.OrderIndex ?? int.MaxValue).ThenBy(a => a.Id)
                        .Select(a => new TestAnswerDto
                        {
                            Id = a.Id,
                            AnswerText = a.AnswerText,
                            IsCorrect = a.IsCorrect
                        }).ToList()
                }).ToListAsync();
        }

        public async Task AddQuestionAsync(int testId, string questionText)
        {
            _db.Questions.Add(new Question { TestId = testId, QuestionText = questionText });
            await _db.SaveChangesAsync();
        }

        public async Task UpdateQuestionAsync(int id, string questionText)
        {
            var q = await _db.Questions.FindAsync(id);
            if (q is null) return;
            q.QuestionText = questionText;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteQuestionAsync(int id)
        {
            var q = await _db.Questions.FindAsync(id);
            if (q is null) return;
            _db.Questions.Remove(q);
            await _db.SaveChangesAsync();
        }

        // ── Answers CRUD ─────────────────────────────────────────────────────────

        public async Task AddAnswerAsync(int questionId, string answerText, bool isCorrect)
        {
            _db.Answers.Add(new Answer { QuestionId = questionId, AnswerText = answerText, IsCorrect = isCorrect });
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAnswerAsync(int id, string answerText, bool isCorrect)
        {
            var a = await _db.Answers.FindAsync(id);
            if (a is null) return;
            a.AnswerText = answerText;
            a.IsCorrect = isCorrect;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAnswerAsync(int id)
        {
            var a = await _db.Answers.FindAsync(id);
            if (a is null) return;
            _db.Answers.Remove(a);
            await _db.SaveChangesAsync();
        }
    }
}

using DiplomProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace DiplomProject.Services
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
    }
}

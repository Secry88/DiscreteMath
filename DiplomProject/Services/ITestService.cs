using DiscreteMath.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace DiscreteMath.Services
{
    public interface ITestService
    {
        Task<List<TestSelectionItemDto>> GetTestsForSelectionAsync(int userId);
        Task<TestDetailDto?> GetTestDetailAsync(int testId);
        Task SaveTestResultAsync(int userId, int testId, int score, int totalQuestions);
    }
}

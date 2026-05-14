using DiscreteMath.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscreteMath.Services
{
    public interface IPracticeService
    {
        Task<List<PracticeTaskDto>> GetPracticeTasksAsync();
    }
}

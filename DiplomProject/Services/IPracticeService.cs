using DiplomProject.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiplomProject.Services
{
    public interface IPracticeService
    {
        Task<List<PracticeTaskDto>> GetPracticeTasksAsync();
    }
}

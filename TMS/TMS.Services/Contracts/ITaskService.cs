
using TMS.Services.Models;

namespace TMS.Services.Contracts
{
    public interface ITaskService
    {
        Task<List<TMS.Services.Models.TaskVM>> GetAllTasksAsync();
        Task<TMS.Services.Models.TaskVM> GetTaskByIdAsync(string taskId);
        Task<TMS.Services.Models.TaskVM> CreateTaskAsync(TMS.Data.Models.Task task);
        Task UpdateTaskAsync(string taskId, TMS.Services.Models.TaskUM task);
        Task DeleteTaskAsync(string taskId);
        Task<List<TaskVM?>> GetTaskByNameAsync(string taskName);
        Task AssignTaskToUserAsync(string taskId, string userId);
        Task AssignTaskToGroupAsync(string taskId, string groupId);
        Task<List<TaskVM?>> GetTaskByCategoryAsync(string searchValue);
        Task<List<TaskVM?>> GetTaskByPriorityAsync(string searchValue);
        Task<List<TaskVM?>> GetTaskByStatusAsync(string searchValue);
        Task MarkTaskAsComplete(string taskId);
    }
}

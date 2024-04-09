using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;
using TMS.Data.Data;
using TMS.Services.Contracts;
using TMS.Services.Models;

namespace TMS.Services.Implementations
{
    public class TaskService : ITaskService
    {
        private readonly TMSContext _context;
        private readonly IMapper _mapper;

        public TaskService(TMSContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<TaskVM> CreateTaskAsync(TMS.Data.Models.Task task)
        {
            task.TaskId = Guid.NewGuid().ToString();
            task.CreatedOn = DateTime.Now;
            task.Status = Data.Enums.TaskStatus.InProgress;

            _context.Tasks.Add(task);

            await _context.SaveChangesAsync();

            return _mapper.Map<TaskVM>(task);
        }
        public async Task DeleteTaskAsync(string taskId)
        {
            var task = await _context
                .Tasks
                .FindAsync(taskId);

            if (task == null)
            {
                throw new ArgumentException("Task not found");
            }

            _context.Tasks.Remove(task);

            await _context.SaveChangesAsync();
        }
        public async Task<List<TaskVM>> GetAllTasksAsync()
        {
            var tasks = await _context
                .Tasks
                .ProjectTo<TaskVM>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return tasks;
        }
        public async Task<TaskVM> GetTaskByIdAsync(string taskId)
        {
            var task = await _context
                .Tasks
                .Where(t => t.TaskId == taskId)
                .ProjectTo<TaskVM>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

                return task;
        }

        public async Task<List<TaskVM?>> GetTaskByNameAsync(string taskName)
        {
            var task = await _context
                .Tasks
                .Where(t => t.Name == taskName)
                .ProjectTo<TaskVM>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return task;
        }

        public async Task UpdateTaskAsync(string taskId, TaskUM task)
        {
            var taskToUpdate = await _context
                .Tasks
                .FindAsync(taskId);

            taskToUpdate.Name = task.Name;
            taskToUpdate.Description = task.Description;
            taskToUpdate.StartDate = task.StartDate;
            taskToUpdate.DueDate = task.DueDate;
            taskToUpdate.Category = task.Category;
            taskToUpdate.Priority = task.Priority;
            taskToUpdate.Status = task.Status;

            await _context.SaveChangesAsync();
        }

        public async Task AssignTaskToUserAsync(string taskId, string userId)
        {
            var taskToAssign = await _context
                .Tasks
                .FindAsync(taskId);

            taskToAssign.UserId = userId;

            await _context.SaveChangesAsync();
        }

        public async Task AssignTaskToGroupAsync(string taskId, string groupId)
        {
            var taskToAssign = await _context
                .Tasks
                .FindAsync(taskId);

            taskToAssign.GroupId = groupId;

            await _context.SaveChangesAsync();
        }

        public async Task<List<TaskVM?>> GetTaskByCategoryAsync(string searchValue)
        {

            var category = (TMS.Data.Enums.TaskCategory)Enum.Parse(typeof(TMS.Data.Enums.TaskCategory), searchValue);
            var task = await _context
                .Tasks
                .Where(t => t.Category == category)
                .ProjectTo<TaskVM>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return task;
        }

        public async Task<List<TaskVM?>> GetTaskByPriorityAsync(string searchValue)
        {
            var priority = (TMS.Data.Enums.TaskPriority)Enum.Parse(typeof(TMS.Data.Enums.TaskPriority), searchValue);
            
            var task = await _context
                .Tasks
                .Where(t => t.Priority == priority)
                .ProjectTo<TaskVM>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return task;
        }

        public async Task<List<TaskVM?>> GetTaskByStatusAsync(string searchValue)
        {
            var status = (TMS.Data.Enums.TaskStatus)Enum.Parse(typeof(TMS.Data.Enums.TaskStatus), searchValue);

            var task = await _context
                .Tasks
                .Where(t => t.Status == status)
                .ProjectTo<TaskVM>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return task;
        }

        public async Task MarkTaskAsComplete(string taskId)
        {
            var task = await _context
                .Tasks
                .FindAsync(taskId);

            task.Status = Data.Enums.TaskStatus.Completed;

            await _context.SaveChangesAsync();
        }
    }
}

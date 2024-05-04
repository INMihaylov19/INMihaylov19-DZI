using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Threading.Tasks;
using TMS.Data.Models;
using TMS.Services.Contracts;
using TMS.Services.Implementations;
using TMS.Services.Models;
using TMS.WebHost.Models;

namespace TMS.WebHost.Controllers
{
    public class TaskController : Controller
    {
        private readonly ITaskService _taskService;
        private readonly IUserService _userService;
        private readonly IGroupService _groupService;
        private readonly IMapper _mapper;
        private readonly IPDFDownloader _pdfDownloader;
        private readonly IEmailSender _emailSender;

        public TaskController(ITaskService taskService,
            IMapper mapper,
            IUserService userService,
            IGroupService groupService,
            IPDFDownloader pdfDownloader,
            IEmailSender emailSender)
        {
            _taskService = taskService;
            _mapper = mapper;
            _userService = userService;
            _groupService = groupService;
            _pdfDownloader = pdfDownloader;
            _emailSender = emailSender;
        }

        // GET: TasksController
        [Authorize(Policy = "RequiredExEmployeeEmployer")]
        public async Task<IActionResult> Index(string searchBy, string searchValue)
        {
            var tasks = await _taskService.GetAllTasksAsync();

            try
            {
                if (tasks == null)
                {
                    TempData["Info"] = "Няма задачи.";
                }
                else
                {
                    if (string.IsNullOrEmpty(searchValue))
                    {
                        return View(tasks);
                    }
                    else
                    {

                        switch (searchBy.ToLower())
                        {
                            case "taskname":
                                var searchByTaskName = await _taskService.GetTaskByNameAsync(searchValue);
                                return View(searchByTaskName);
                            case "category":
                                var searchByCategory = await _taskService.GetTaskByCategoryAsync(searchValue);
                                return View(searchByCategory);
                            case "priority":
                                var searchByPriority = await _taskService.GetTaskByPriorityAsync(searchValue);
                                return View(searchByPriority);
                            case "status":
                                var searchByStatus = await _taskService.GetTaskByStatusAsync(searchValue);
                                return View(searchByStatus);
                            default:
                                return View(tasks);
                        }
                    }
                }
                return View(tasks);
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        // GET: TasksController/Details/5
        public async Task<IActionResult> Details(string id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            return View(task);
        }

        // GET: TasksController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TasksController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequiredEmployer")]
        public async Task<IActionResult> Create(TMS.WebHost.Models.TaskIM task)
        {
            try
            {
                if (task.StartDate < DateTime.Today)
                {
                    TempData["Error"] = "Началната дата не може да бъде в миналото.";
                    return View(task);
                }

                if (task.DueDate < task.StartDate)
                {
                    TempData["Error"] = "Крайната дата не може да бъде преди началната.";
                    return View(task);
                }
                await _taskService.CreateTaskAsync(_mapper.Map<TMS.Data.Models.Task>(task));

                return RedirectToAction("Index");
            }
            catch
            {
                return View(task);
            }
        }

        // GET: TasksController/Edit/5
        [HttpGet]
        [Authorize(Policy = "RequiredEmployer")]
        public async Task<IActionResult> Edit(string id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);

            if (task == null)
            {
                return View("Error");
            }

            return View(_mapper.Map<TaskUM>(task));
        }

        // POST: TasksController/Edit/5
        [HttpPost]
        [Authorize(Policy = "RequiredEmployer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, TMS.Services.Models.TaskUM taskUM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Invalid task");
                return View("Edit", taskUM);
            }
            try
            {
                await _taskService.UpdateTaskAsync(id, taskUM);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(taskUM);
            }
        }
        [HttpGet]
        [Authorize(Policy = "RequiredEmployer")]

        // GET: TasksController/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null)
            {
                return View("Error");
            }
            return View(task);
        }

        // POST: TasksController/Delete/5
        [HttpPost]
        [Authorize(Policy = "RequiredEmployer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed([FromForm] string taskId)
        {
            try
            {
                await _taskService.DeleteTaskAsync(taskId);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View("Error");
            }
        }

        [HttpGet]
        [Authorize(Policy = "RequiredEmployer")]
        public async Task<IActionResult> AssignTaskToUser(string id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            var users = await _userService.GetAllUsersAsync();
            if (task == null)
            {
                return View("Error");
            }

            var viewModel = new AssignTaskToUserVM
            {
                Task = task,
                Users = users
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Policy = "RequiredEmployer")]
        public async Task<IActionResult> AssignTaskToUser(string id, AssignTaskToUserVM viewModel)
        {
            try
            {
                await _taskService.AssignTaskToUserAsync(id, viewModel.Task.UserId);
                var assignedUser = await _userService.GetUserByIdAsync(viewModel.Task.UserId);
                await _emailSender.SendEmailAsync(assignedUser.Email, "Възложена задача", "Вие получихте нова задача.");
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                TempData["Error"] = "Възникна грешка, моля опитайте отново!";
                return View(viewModel);
            }
        }

        [HttpGet]
        [Authorize(Policy = "RequiredEmployer")]
        public async Task<IActionResult> AssignTaskToGroup(string id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            var groups = await _groupService.GetAllGroupsAsync();
            if (task == null)
            {
                return View("Error");
            }

            var viewModel = new AssignTaskToGroupVM
            {
                Task = task,
                Groups = groups
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Policy = "RequiredEmployer")]
        public async Task<IActionResult> AssignTaskToGroup(string id, AssignTaskToGroupVM viewModel)
        {
            try
            {
                await _taskService.AssignTaskToGroupAsync(id, viewModel.Task.GroupId);
                return RedirectToAction("Index");
            }
            catch
            {
                return View("Error");
            }
        }

        public async Task<IActionResult> DownloadPDF(string id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            var user = await _userService.GetUserByIdAsync(task.UserId);
            var group = await _groupService.GetGroupByIdAsync(task.GroupId);

            string htmlContent = $@"
            <table class='table'>
                <tr>
                    <th>Name</th>
                    <td>{task.Name}</td>
                </tr>
                <tr>
                    <th>Description</th>
                    <td>{task.Description}</td>
                </tr>
                <tr>
                    <th>Start Date</th>
                    <td>{task.StartDate}</td>
                </tr>
                <tr>
                    <th>Due Date</th>
                    <td>{task.DueDate}</td>
                </tr>
                <tr>
                    <th>User</th>
                    <td>{user?.Username}</td>
                </tr>
                <tr>
                    <th>Group</th>
                    <td>{group?.GroupName}</td>
                </tr>
                <tr>
                    <th>Category</th>
                    <td>{task.Category}</td>
                </tr>
                <tr>
                    <th>Priority</th>
                    <td>{task.Priority}</td>
                </tr>
                <tr>
                    <th>Status</th>
                    <td>{task.Status}</td>
                </tr>
                <tr>
                    <th>Created On</th>
                    <td>{task.CreatedOn}</td>
                </tr>
            </table>";
            var bytes = await _pdfDownloader.DownloadPDF(htmlContent);
            return File(bytes, "application/pdf", $"{task.Name}.pdf");
        }

        [HttpPost]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> MarkTaskAsComplete(string taskId)
        {
            try
            {
                await _taskService.MarkTaskAsComplete(taskId);
                return RedirectToAction("GetTasksForCurrentUser", "User"); 
            }
            catch
            {
                return View("Error");
            }
        }
    }
}

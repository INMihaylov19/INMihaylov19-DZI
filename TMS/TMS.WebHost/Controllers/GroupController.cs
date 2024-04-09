using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using TMS.Services.Contracts;
using TMS.Services.Models;
using TMS.WebHost.Models;

namespace TMS.WebHost.Controllers
{
    public class GroupController : Controller
    {

        private readonly IGroupService _groupService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public GroupController(IGroupService groupService,
            IMapper mapper,
            IUserService userService)
        {
            _groupService = groupService;
            _mapper = mapper;
            _userService = userService;
        }

        [Authorize(Policy = "RequiredEmployer")]
        public async Task<IActionResult> Index(string searchBy, string searchValue)
        {
            try
            {
                var groups = await _groupService.GetAllGroupsAsync();
                if (groups == null)
                {
                    TempData["Info"] = "There are no groups.";
                }
                else
                {
                    if(string.IsNullOrEmpty(searchValue))
                    {
                        return View(groups);
                    }
                    else
                    {
                        if (searchBy.ToLower() == "groupname")
                        {
                           var searchByGroupName = await _groupService.GetGroupdByNameAsync(searchValue);
                           return View(searchByGroupName);
                        }
                    }
                }
                return View(groups);
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        // GET: GroupsController/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            var group = await _groupService.GetGroupByIdAsync(id);
            return View(group);
        }

        // GET: GroupsController/Create
        [Authorize(Policy = "RequiredEmployer")]
        public IActionResult Create()
        {

            return View();
        }

        // POST: GroupsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequiredEmployer")]
        public async Task<IActionResult> Create(TMS.WebHost.Models.GroupIM group)
        {
            try
            {
                if (group.GroupName == null)
                {
                    TempData["Error"] = "Името е задължително";
                    return View(group);
                }
                await _groupService.CreateGroupAsync(_mapper.Map<TMS.Data.Models.Group>(group));

                return RedirectToAction("Index");

            }
            catch
            {
                return View(group);

            }
        }

        // GET: GroupsController/Edit/5
        [HttpGet]
        [Authorize(Policy = "RequiredEmployer")]
        public async Task<IActionResult> Edit(string id)
        {
            var group = await _groupService.GetGroupByIdAsync(id);

            if (group == null)
            {
                return View("Error");
            }

            return View(_mapper.Map<GroupUM>(group));
        }

        // POST: GroupsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequiredEmployer")]
        public async Task<IActionResult> Edit(string id, TMS.Services.Models.GroupUM groupUM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Invalid group");
                return View("Edit", groupUM);
            }

            try
            {
                await _groupService.UpdateGroupAsync(id, groupUM);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(groupUM);
            }
        }

        // GET: GroupsController/Delete/5
        [HttpGet]
        [Authorize(Policy = "RequiredEmployer")]
        public async Task<IActionResult> Delete(string id)
        {
            var group = await _groupService.GetGroupByIdAsync(id);
            if (group == null)
            {
                return View("Error");
            }
            return View(group);
        }

        // POST: GroupsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequiredEmployer")]
        public async Task<IActionResult> DeleteConfirmed([FromForm] string groupId)
        {
            try
            {
                await _groupService.DeleteGroupAsync(groupId);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View("Error");
            }

        }

        [HttpPost]
        [Authorize(Policy = "RequiredEmployer")]
        public async Task<IActionResult> AddUserToGroup(string groupId, string userId)
        {
            try
            {
                await _groupService.AssignUserToGroupAsync(groupId, userId);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View("Error");
            }
        }

        [HttpGet]
        [Authorize(Policy = "RequiredEmployer")]
        public async Task<IActionResult> AssignUserToGroup(string id)
        {
            var group = await _groupService.GetGroupByIdAsync(id);
            var users = await _userService.GetAllUsersAsync();

            if (group == null)
            {
                return View("Error");
            }

            var viewModel = new AssignUserToGroupVM
            {
                Group = group,
                Users = users
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Policy = "RequiredEmployer")]
        public async Task<IActionResult> AssignUserToGroup(string id, AssignUserToGroupVM viewModel)
        {
            try
            {
                await _groupService.AssignUserToGroupAsync(viewModel.Group.UserId, id);
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                return View("Error");
            }
        }

        [HttpGet]
        [Authorize(Policy = "RequiredEmployer")]
        public async Task<IActionResult> RemoveUserFromGroup(string id)
        {
            var group = await _groupService.GetGroupByIdAsync(id);
            var users = await _userService.GetAllUsersAsync();

            if (group == null)
            {
                return View("Error");
            }

            var removeUserFromGroupVM = new RemoveUserFromGroupVM
            {
                Group = group,
                Users = users
            };

            return View(removeUserFromGroupVM);
        }

        [HttpPost]
        [Authorize(Policy = "RequiredEmployer")]
        public async Task<IActionResult> RemoveUserFromGroup(string id, RemoveUserFromGroupVM viewModel)
        {
            try
            {
                await _groupService.RemoveUserFromGroup(viewModel.Group.UserId, id); 
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                return View("Error");
            }
        }   
    }
}

using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using TMS.Services.Contracts;
using TMS.Services.Models;

namespace TMS.WebHost.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly UserManager<TMS.Data.Models.User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly IImageService _imageService;

        public UserController(IUserService userService,
            IMapper mapper,
            UserManager<TMS.Data.Models.User> userManager,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender,
            IImageService imageService)
        {
            _userService = userService;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
            _imageService = imageService;
        }
        // GET: UsersController
        [Authorize(Policy = "RequiredAdminEmployer")]
        public async Task<IActionResult> Index(string searchBy, string searchValue)
        {
            var users = await _userService.GetAllUsersAsync();

            try
            {
                if (users == null)
                {
                    TempData["Info"] = "Няма потребители.";
                }
                else
                {
                    if (string.IsNullOrEmpty(searchValue))
                    {
                        return View(users);
                    }
                    else
                    {
                        switch (searchBy.ToLower())
                        {
                            case "username":
                                var searchByUserName = await _userService.GetUserByUsernameAsync(searchValue);
                                return View(searchByUserName);
                            case "firstname":
                                var searchByFirstName = await _userService.GetUserByFirstNameAsync(searchValue);
                                return View(searchByFirstName);
                            case "role":
                                var searchByRole = await _userService.GetUserByRoleAsync(searchValue);
                                return View(searchByRole);
                            default:
                                return View(users);
                        }
                    }
                }
                return View(users);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View();
            }
            return View(users);
        }

        // GET: UsersController/Details/5
        public async Task<IActionResult> Details(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return View(user);
        }

        // GET: UsersController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UsersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequiredAdmin")]
        public async Task<IActionResult> Create(TMS.WebHost.Models.UserIM userIM)
        {
            try
            {
                var imageId = await _imageService.UploadImageAsync(userIM.Image, userIM.Username);

                var user = new TMS.Data.Models.User
                {
                    FirstName = userIM.FirstName,
                    LastName = userIM.LastName,
                    UserName = userIM.Username,
                    Email = userIM.Email,
                    Role = userIM.Role,
                    CreatedOn = DateTime.Now,
                    ImageId = imageId
                };


                user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, userIM.Password);
                var adminRoleExists = await _roleManager.RoleExistsAsync(nameof(TMS.Data.Enums.UserRole.Admin));
                var employeeRoleExists = await _roleManager.RoleExistsAsync(nameof(TMS.Data.Enums.UserRole.Employee));
                var employerRoleExists = await _roleManager.RoleExistsAsync(nameof(TMS.Data.Enums.UserRole.Employer));
                var exEmployeeRoleExists = await _roleManager.RoleExistsAsync(nameof(TMS.Data.Enums.UserRole.ExEmployee));

                if (!adminRoleExists)
                {
                    await _roleManager.CreateAsync(new IdentityRole(nameof(TMS.Data.Enums.UserRole.Admin)));
                }

                if (!employeeRoleExists)
                {
                    await _roleManager.CreateAsync(new IdentityRole(nameof(TMS.Data.Enums.UserRole.Employee)));
                }

                if (!employerRoleExists)
                {
                    await _roleManager.CreateAsync(new IdentityRole(nameof(TMS.Data.Enums.UserRole.Employer)));
                }

                if (!exEmployeeRoleExists)
                {
                    await _roleManager.CreateAsync(new IdentityRole(nameof(TMS.Data.Enums.UserRole.ExEmployee)));
                }

                var userExists = await _userManager.FindByEmailAsync(user.Email);

                if (userExists != null)
                {
                    TempData["Error"] = "Потребител с този имейл вече съществува.";
                    return View(userIM);
                }

                var result = await _userManager.CreateAsync(user, userIM.Password);

                if (!result.Succeeded)
                {
                    var error = result.Errors.FirstOrDefault().Description;

                    TempData["Error"] = "Паролата трябва да съдържа една главна буква, една малка буква и един специален символ. Моля, опитайте отново";
                    return View(userIM);
                }

                switch (user.Role)
                {
                    case TMS.Data.Enums.UserRole.Admin:
                        await _userManager.AddToRoleAsync(user, nameof(TMS.Data.Enums.UserRole.Admin));
                        var claimAdmin = new Claim("Permission", "IsAdmin");
                        var role = await _roleManager.FindByNameAsync(nameof(TMS.Data.Enums.UserRole.Admin));
                        if (!_userService.HasClaim(claimAdmin.Type, claimAdmin.Value))
                        {
                            await _roleManager.AddClaimAsync(role, claimAdmin);
                        }
                        break;
                    case TMS.Data.Enums.UserRole.Employee:
                        await _userManager.AddToRoleAsync(user, nameof(TMS.Data.Enums.UserRole.Employee));
                        var claimEmployee = new Claim("Permission", "IsEmployee");
                        var roleEmployee = await _roleManager.FindByNameAsync(nameof(TMS.Data.Enums.UserRole.Employee));
                        if (!_userService.HasClaim(claimEmployee.Type, claimEmployee.Value))
                        {
                            await _roleManager.AddClaimAsync(roleEmployee, claimEmployee);
                        }
                        break;
                    case TMS.Data.Enums.UserRole.Employer:
                        await _userManager.AddToRoleAsync(user, nameof(TMS.Data.Enums.UserRole.Employer));
                        var claimEmployer = new Claim("Permission", "IsEmployer");
                        var roleEmployer = await _roleManager.FindByNameAsync(nameof(TMS.Data.Enums.UserRole.Employer));
                        if (!_userService.HasClaim(claimEmployer.Type, claimEmployer.Value))
                        {
                            await _roleManager.AddClaimAsync(roleEmployer, claimEmployer);
                        }
                        break;
                   case TMS.Data.Enums.UserRole.ExEmployee:
                        await _userManager.AddToRoleAsync(user, nameof(TMS.Data.Enums.UserRole.ExEmployee));
                        var claimExEmployee = new Claim("Permission", "IsExEmployee");
                        var roleExEmployee = await _roleManager.FindByNameAsync(nameof(TMS.Data.Enums.UserRole.ExEmployee));
                        if (!_userService.HasClaim(claimExEmployee.Type, claimExEmployee.Value))
                        {
                            await _roleManager.AddClaimAsync(roleExEmployee, claimExEmployee);
                        }
                        break;
                    default:
                        break;
                }

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, HttpContext.Request.Scheme);

                var emailBody = "Моля потвърдете вашият имейл като натиснете " + $"<a href=\"{confirmationLink}\">тук</a>";

                await _emailSender.SendEmailAsync(user.Email, "Потвърдете своя имейл", emailBody);

                return RedirectToAction("Index");
            }
            catch
            {
                return View(userIM);
            }
        }

        // GET: UsersController/Edit/5
        [HttpGet]
        [Authorize(Policy = "RequiredEmployeeAdminEmployer")]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return View("Error");
            }

            return View(_mapper.Map<UserUM>(user));
        }

        // POST: UsersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequiredEmployeeAdminEmployer")]
        public async Task<IActionResult> Edit(string id, TMS.Services.Models.UserUM userUM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Invalid user");
                return View("Edit", userUM);
            }


            try
            {
                var user = _userService.GetUserByIdAsync(id);
                userUM.Role = user.Result.Role;
                await _userService.UpdateUserAsync(id, userUM);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(userUM);
            }
        }

        [HttpGet]
        [Authorize(Policy = "RequiredAdmin")]
        // GET: UsersController/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return View("Error");
            }
            return View(user);
        }

        // POST: UsersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequiredAdmin")]
        public async Task<IActionResult> DeleteConfirmed([FromForm] string id)
        {
            try
            {
                await _userService.DeleteUserAsync(id);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View("Error");
            }
        }

        [HttpGet]
        [Authorize(Policy = "RequiredEmployee")]
        public IActionResult GetTasksForCurrentUser()
        {
            var user = _userService.GetTasksFofCurrentUserAsync();

            if (user == null)
            {
                return View("Error");
            }
            return View(user);
        }

        [HttpGet]
        [Authorize(Policy = "RequiredEmployee")]
        public IActionResult GetGroupsOfCurrentUser()
        {
            var user = _userService.GetGroupsOfCurrentUser();

            if (user == null)
            {
                return View("Error");
            }
            return View(user);
        }

        [HttpGet]
        [Authorize(Policy = "RequiredAdminEmployer")]
        public async Task<IActionResult> DaysSinceRegistration(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            return View(user);
        }

        public async Task<IActionResult> GetImage(string imageId)
        {
            try
            {
                var image = await _imageService.GetImageDataAsync(imageId);
                return File(image, "image/jpeg");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View();
            }
        }
    }
}

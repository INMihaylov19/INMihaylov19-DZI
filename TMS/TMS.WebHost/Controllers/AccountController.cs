using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Security.Claims;
using TMS.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using TMS.Data.Models;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore;
using TMS.Data.Data;
using TMS.WebHost.Models.Account;
using System.Security.Claims;
using TMS.Data.Enums;

namespace TMS.WebHost.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<TMS.Data.Models.User> _userManager;
        private readonly IUserService _userService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<TMS.Data.Models.User> _signInManager;
        private readonly IMapper _mapper;

        public AccountController(UserManager<TMS.Data.Models.User> userManager,
            IUserService userService,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender,
            SignInManager<Data.Models.User> signInManager,
            IMapper mapper)
        {
            _userManager = userManager;
            _userService = userService;
            _roleManager = roleManager;
            _emailSender = emailSender;
            _signInManager = signInManager;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            var response = new LoginVM();
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }

            var user = await _userService.GetUserByEmailAsync(loginVM.Email);

            if (user != null)
            {
                if (!user.EmailConfirmed)
                {
                    TempData["Error"] = "Моля първо потвърдете имейл адреса си.";
                    return View(loginVM);
                }

                var passwordCheck = await _userManager.CheckPasswordAsync(user, loginVM.Password);

                if (passwordCheck)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                TempData["Error"] = "Грешни идентификационни данни. Моля, опитайте отново.";
                return View(loginVM);
            }

            TempData["Error"] = "Грешни идентификационни данни. Моля, опитайте отново.";
            return View(loginVM);

        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            var response = new RegisterVM();
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View(registerVM);
            }

            var user = await _userService.GetUserByEmailAsync(registerVM.Email);

            if (user != null)
            {
                TempData["Error"] = "Този имейл адрес вече е използван.";
                return View(registerVM);
            }

            var newUser = new TMS.Data.Models.User()
            {
                FirstName = registerVM.FirstName,
                LastName = registerVM.LastName,
                Email = registerVM.Email,
                UserName = registerVM.UserName,
            };

            var isUserFirst = _userService.GetAllUsersAsync().Result.Count == 0;

            if (isUserFirst)
            {
                newUser.Role = Data.Enums.UserRole.Admin;
            }
            else
            {
                newUser.Role = Data.Enums.UserRole.Employee;
            }

            var newUserResponse = await _userManager.CreateAsync(newUser, registerVM.Password);

            if (!newUserResponse.Succeeded)
            {
                var error = newUserResponse.Errors.FirstOrDefault();
                TempData["Error"] = $"Възникна грешка. Моля, опитайте отново.";

                return View(registerVM);
            }

            if (isUserFirst)
            {
                var adminRoleExists = await _roleManager.RoleExistsAsync(nameof(TMS.Data.Enums.UserRole.Admin));
                if (!adminRoleExists)
                {
                    await _roleManager.CreateAsync(new IdentityRole(nameof(TMS.Data.Enums.UserRole.Admin)));
                }
                await _userManager.AddToRoleAsync(newUser, nameof(TMS.Data.Enums.UserRole.Admin));
            }
            else
            {
                var roleExists = await _roleManager.RoleExistsAsync(nameof(TMS.Data.Enums.UserRole.Employee));
                if (!roleExists)
                {
                    await _roleManager.CreateAsync(new IdentityRole(nameof(TMS.Data.Enums.UserRole.Employee)));
                }
                await _userManager.AddToRoleAsync(newUser, nameof(TMS.Data.Enums.UserRole.Employee));
            }


            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, newUser.Email),
                new Claim(ClaimTypes.Role, newUser.Role.ToString())
            };

            await _userManager.AddClaimsAsync(newUser, userClaims);

            var roleAdmin = await _roleManager.FindByNameAsync(nameof(UserRole.Admin));
            var roleEmployee = await _roleManager.FindByNameAsync(nameof(UserRole.Employee));

            if (newUser.Role == Data.Enums.UserRole.Admin)
            {
                var claim = new Claim("Permission", "IsAdmin");
                if (!_userService.HasClaim(claim.Type, claim.Value))
                {
                    await _roleManager.AddClaimAsync(roleAdmin, claim);
                }
            }

            else if (newUser.Role == Data.Enums.UserRole.Employee)
            {
                var claim = new Claim("Permission", "IsEmployee");
                if (!_userService.HasClaim(claim.Type, claim.Value))
                {
                    await _roleManager.AddClaimAsync(roleEmployee, claim);
                }
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);

            var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = newUser.Id, token }, protocol: HttpContext.Request.Scheme);

            var emailBody = "Моля потвърдете своя имейл като натиснете " + $"<a href=\"{confirmationLink}\">тук</a>";

            try
            {
                await _emailSender.SendEmailAsync(newUser.Email, "Потвърждаване на имейл", emailBody);
            }
            catch
            {
                TempData["Error"] = "Възникна грешка при изпращане на имейл. Моля опитайте по-късно";
                return View(registerVM);
            }

            return RedirectToAction("RegisterConfirmation", "Account");
        }

        public IActionResult RegisterConfirmation()
        {
            return View();
        }

        public IActionResult EmailConfirmation()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return RedirectToAction("Error");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                return RedirectToAction("EmailConfirmation", "Account");
            }

            return RedirectToAction("Error");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM forgotPasswordVM)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.GetUserByEmailAsync(forgotPasswordVM.Email);

                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var resetLink = Url.Action("ResetPassword", "Account", new { email = user.Email, token }, protocol: HttpContext.Request.Scheme);

                    var emailBody = "Please reset your password by clicking " + $"<a href=\"{resetLink}\">here</a>";
                    await _emailSender.SendEmailAsync(user.Email, "Password Reset", emailBody);

                    return RedirectToAction("ForgotPasswordConfirmation", "Account");
                }

                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            return View(forgotPasswordVM);
        }

        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            var model = new ResetPasswordVM { Email = email, Token = token };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM resetPasswordVM)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.GetUserByEmailAsync(resetPasswordVM.Email);

                if (user != null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, resetPasswordVM.Token, resetPasswordVM.Password);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("ResetPasswordConfirmation");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                else
                {
                    return RedirectToAction("Error");
                }
            }

            return View(resetPasswordVM);
        }

        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Authorize(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return View("Error");
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> AuthorizeConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                var roleExists = await _roleManager.RoleExistsAsync(nameof(TMS.Data.Enums.UserRole.Employer));
                if (!roleExists)
                {
                    await _roleManager.CreateAsync(new IdentityRole(nameof(TMS.Data.Enums.UserRole.Employer)));
                }

                var userRoles = await _userManager.GetRolesAsync(user);

                if (!userRoles.Contains(nameof(TMS.Data.Enums.UserRole.Employer)))
                {
                    await _userManager.RemoveFromRoleAsync(user, nameof(TMS.Data.Enums.UserRole.Employee));
                    await _userManager.AddToRoleAsync(user, nameof(TMS.Data.Enums.UserRole.Employer));
                }

                user.Role = Data.Enums.UserRole.Employer;

                var role = await _roleManager.FindByNameAsync(nameof(TMS.Data.Enums.UserRole.Employer));

                var claim = new Claim("Permission", "IsEmployer");
                await _roleManager.AddClaimAsync(role, claim);

                await _userService.UpdateUserAsync(id, _mapper.Map<TMS.Services.Models.UserUM>(user));

                return RedirectToAction("Index", "User");
            }

            return View(user);

        }
    }
}

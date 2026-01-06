using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pronia.ViewModels.UserViewModels;
using System.Threading.Tasks;

namespace Pronia.Controllers
{
    public class AccountController(UserManager<AppUser> _userManager, SignInManager<AppUser> _signInManager, RoleManager<IdentityRole> _roleManager, IConfiguration _configuration) : Controller
    {
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var existUser = await _userManager.FindByNameAsync(vm.UserName);

            if (existUser is { })
            {
                ModelState.AddModelError("UserName", "This username is already exist~");
                return View(vm);
            }

            existUser = await _userManager.FindByEmailAsync(vm.EmailAddress);
            if (existUser is { })
            {
                ModelState.AddModelError("EmailAddress", "This email is already exist!");
                return View(vm);
            }

            AppUser appUser = new AppUser()
            {
                FullName = vm.FirstName + " " + vm.LastName,
                UserName = vm.UserName,
                Email = vm.EmailAddress
            };

            var result = await _userManager.CreateAsync(appUser, vm.Password);

            if (result.Succeeded == false)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(vm);
            }
            await _userManager.AddToRoleAsync(appUser, "Member");
            await _signInManager.SignInAsync(appUser, false);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var user = await _userManager.FindByEmailAsync(vm.EmailAddress);

            if (user is null)
            {
                ModelState.AddModelError("", "Email or password is wrong");
                return View(vm);
            }

            var result = await _userManager.CheckPasswordAsync(user, vm.Password);

            if (result == false)
            {
                ModelState.AddModelError("", "Email or password is wrong");
                return View(vm);
            }

            await _signInManager.SignInAsync(user, vm.isRemember);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        public async Task<IActionResult> CreateRoles()
        {
            await _roleManager.CreateAsync(new IdentityRole()
            {
                Name = "Admin"
            });
            await _roleManager.CreateAsync(new IdentityRole()
            {
                Name = "Moderator"
            });
            await _roleManager.CreateAsync(new IdentityRole()
            {
                Name = "Member"
            });

            return Ok("Roles created successfully!");
        }

        public async Task<IActionResult> CreateAdminAndModerator()
        {
            var adminUserVM = _configuration.GetSection("AdminUser").Get<UserVM>();
            var moderatorUserVM = _configuration.GetSection("ModeratorUser").Get<UserVM>();

            if (adminUserVM is not null)
            {
                AppUser adminUser = new AppUser()
                {
                    FullName = adminUserVM.FullName,
                    Email = adminUserVM.Email,
                    UserName = adminUserVM.UserName,
                };
                await _userManager.CheckPasswordAsync(adminUser, adminUserVM.Password);
                await _userManager.AddToRoleAsync(adminUser, "Admin");
            }

            if (moderatorUserVM is not null)
            {
                AppUser moderatorUser = new AppUser()
                {
                    FullName = moderatorUserVM.FullName,
                    Email = moderatorUserVM.Email,
                    UserName = moderatorUserVM.UserName,
                };
                await _userManager.CheckPasswordAsync(moderatorUser, moderatorUserVM.Password);
                await _userManager.AddToRoleAsync(moderatorUser, "Moderator");
            }

            return Ok("Successfully!");
        }
    }
}

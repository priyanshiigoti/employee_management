using employee_management.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace employee_management.Controllers
{
    [Authorize(Roles = "User")]

    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }


        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            return View(user);
        }
    }
}

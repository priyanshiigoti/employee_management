using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace employee_management.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

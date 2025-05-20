using employee_management.Database;
using employee_management.Models;
using employee_management.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace employee_management.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public EmployeeController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            ViewBag.Departments = await dbContext.Departments.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddorEditEmployee viewModel)
        {
            var email = viewModel.Email?.Trim().ToLower();
            var phone = viewModel.Phone?.Trim();

            bool exists = await dbContext.Employees.AnyAsync(e =>
                e.Email.ToLower() == email || e.Phone == phone);

            if (exists)
            {
                ViewBag.Error = "An employee with the same email or phone number already exists.";
                ViewBag.Departments = await dbContext.Departments.ToListAsync();
                return View(viewModel);
            }

            var employee = new Employee
            {
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                Email = viewModel.Email,
                Phone = viewModel.Phone,
                IsActive = viewModel.IsActive,
                DepartmentId = viewModel.DepartmentId
            };

            await dbContext.Employees.AddAsync(employee);
            await dbContext.SaveChangesAsync();

            return RedirectToAction("List", "Employee");
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var employees = await dbContext.Employees
                .Include(e => e.department)
                .ToListAsync();

            return View(employees);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var employee = await dbContext.Employees.FindAsync(id);
            if (employee == null)
                return RedirectToAction("List", "Employee");

            var viewModel = new AddorEditEmployee
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Phone = employee.Phone,
                IsActive = employee.IsActive,
                DepartmentId = employee.DepartmentId
            };

            ViewBag.Departments = await dbContext.Departments.ToListAsync();
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AddorEditEmployee viewModel)
        {
            var existing = await dbContext.Employees.FindAsync(viewModel.Id);
            if (existing == null)
                return RedirectToAction("List", "Employee");

            var email = viewModel.Email?.Trim().ToLower();
            var phone = viewModel.Phone?.Trim();

            bool duplicate = await dbContext.Employees.AnyAsync(e =>
                e.Id != viewModel.Id &&
                (e.Email.ToLower() == email || e.Phone == phone));

            if (duplicate)
            {
                ViewBag.Error = "Another employee with the same email or phone number already exists.";
                ViewBag.Departments = await dbContext.Departments.ToListAsync();
                return View(viewModel);
            }

            existing.FirstName = viewModel.FirstName;
            existing.LastName = viewModel.LastName;
            existing.Email = viewModel.Email;
            existing.Phone = viewModel.Phone;
            existing.IsActive = viewModel.IsActive;
            existing.DepartmentId = viewModel.DepartmentId;

            await dbContext.SaveChangesAsync();

            return RedirectToAction("List", "Employee");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var employee = await dbContext.Employees.FindAsync(id);
            if (employee != null)
            {
                dbContext.Employees.Remove(employee);
                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("List", "Employee");
        }
    }
}

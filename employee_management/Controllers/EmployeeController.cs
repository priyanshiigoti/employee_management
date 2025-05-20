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
        public async Task<IActionResult> Add(Employee viewModel)
        {
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
            var employees = await dbContext.Employees.Include(e => e.department).ToListAsync();
            return View(employees);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var employee = await dbContext.Employees.FindAsync(id);
            ViewBag.Departments = await dbContext.Departments.ToListAsync();
            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Employee viewModel)
        {
            var employee = await dbContext.Employees.FindAsync(viewModel.Id);
            if (employee != null)
            {
                employee.FirstName = viewModel.FirstName;
                employee.LastName = viewModel.LastName;
                employee.Email = viewModel.Email;
                employee.Phone = viewModel.Phone;
                employee.IsActive = viewModel.IsActive;
                employee.DepartmentId = viewModel.DepartmentId;

                await dbContext.SaveChangesAsync();
            }

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

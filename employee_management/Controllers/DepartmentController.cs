using employee_management.Database;
using employee_management.Models;
using employee_management.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace employee_management.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public DepartmentController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View(new AddorEditDepartment());
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddorEditDepartment viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            if (viewModel.CreatedAt > DateTime.Now)
            {
                ViewBag.Error = "Created date cannot be in the future.";
                return View(viewModel);
            }

            bool exists = await dbContext.Departments
                .AnyAsync(d => d.Name.ToLower() == viewModel.Name.Trim().ToLower());

            if (exists)
            {
                ViewBag.Error = "A department with the same name already exists.";
                return View(viewModel);
            }

            var department = new Department
            {
                Id = Guid.NewGuid(),
                Name = viewModel.Name.Trim(),
                CreatedAt = viewModel.CreatedAt
            };

            await dbContext.Departments.AddAsync(department);
            await dbContext.SaveChangesAsync();

            return RedirectToAction("List");
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var departments = await dbContext.Departments.ToListAsync();
            return View(departments);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var department = await dbContext.Departments.FindAsync(id);
            if (department == null)
                return RedirectToAction("List");

            var viewModel = new AddorEditDepartment
            {
                Id = department.Id,
                Name = department.Name,
                CreatedAt = department.CreatedAt
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AddorEditDepartment viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var existing = await dbContext.Departments.FindAsync(viewModel.Id);
            if (existing == null)
                return RedirectToAction("List");

            bool duplicate = await dbContext.Departments
                .AnyAsync(d => d.Id != viewModel.Id &&
                               d.Name.ToLower() == viewModel.Name.Trim().ToLower());

            if (duplicate)
            {
                ViewBag.Error = "Another department with the same name already exists.";
                return View(viewModel);
            }

            existing.Name = viewModel.Name.Trim();
            existing.CreatedAt = viewModel.CreatedAt;

            await dbContext.SaveChangesAsync();
            return RedirectToAction("List");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var department = await dbContext.Departments.FindAsync(id);
            if (department != null)
            {
                dbContext.Departments.Remove(department);
                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("List");
        }
    }
}

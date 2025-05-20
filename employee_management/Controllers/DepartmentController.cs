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
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Department viewModel)
        {
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
                Name = viewModel.Name,
                CreatedAt = DateTime.Now
            };

            await dbContext.Departments.AddAsync(department);
            await dbContext.SaveChangesAsync();

            return RedirectToAction("List", "Department");
        }


        [HttpGet]
        public async Task<IActionResult> List()
        {
           var Departments =  await dbContext.Departments.ToListAsync();
            return View(Departments);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var Department = await dbContext.Departments.FindAsync(id);
            return View(Department);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Department viewModel)
        {
            var existing = await dbContext.Departments.FindAsync(viewModel.Id);
            if (existing == null)
                return RedirectToAction("List", "Department");

            // Check for duplicate name excluding the current department
            bool duplicate = await dbContext.Departments
                .AnyAsync(d => d.Id != viewModel.Id &&
                               d.Name.ToLower() == viewModel.Name.Trim().ToLower());

            if (duplicate)
            {
                ViewBag.Error = "Another department with the same name already exists.";
                return View(viewModel);
            }

            existing.Name = viewModel.Name.Trim();
            existing.CreatedAt = DateTime.Now;

            await dbContext.SaveChangesAsync();

            return RedirectToAction("List", "Department");
        }

        [HttpPost]
        public async Task <IActionResult>Delete(Department viewModel)
        {
            var Department = await dbContext.Departments
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == viewModel.Id);

            if(Department != null)
            {
                dbContext.Departments.Remove(viewModel);
                await dbContext.SaveChangesAsync();

            }

            return RedirectToAction("List", "Department");
        }
    }
}

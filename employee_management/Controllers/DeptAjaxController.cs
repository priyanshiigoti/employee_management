using employee_management.Database;
using employee_management.Models;
using employee_management.Models.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace employee_management.Controllers
{
    public class DeptAjaxController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public DeptAjaxController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var department = await dbContext.Departments.ToListAsync();
            //return View(department);
            return PartialView("_DepartmentTableRows", department); 

        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Department viewModel)
        {
            if (string.IsNullOrWhiteSpace(viewModel.Name))
            {
                return BadRequest(new { success = false, message = "Department name is required." });
            }

            bool exists = await dbContext.Departments
                .AnyAsync(d => d.Name.ToLower() == viewModel.Name.Trim().ToLower());

            if (exists)
            {
                return Conflict(new { success = false, message = "Department with this name already exists." });
            }

            var department = new Department
            {
                Id = Guid.NewGuid(),
                Name = viewModel.Name.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            dbContext.Departments.Add(department);
            await dbContext.SaveChangesAsync();

            return Ok(new { success = true, message = "Department added successfully." });
        }


        [HttpGet]
        public async Task<IActionResult>Get(Guid id)
        {
            var department = await dbContext.Departments.FindAsync(id);
            if (department == null) 
                return NotFound(new { success = false, message = "Department not found!" });

            return Ok(department);

        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] Department viewModel)
        {
            if (viewModel == null || viewModel.Id == Guid.Empty)
            {
                return BadRequest(new { success = false, message = "Invalid input" });
            }

            var department = await dbContext.Departments.FindAsync(viewModel.Id);
            if (department == null)
            {
                return NotFound(new { success = false, message = "Department Id is not Found" });
            }
            var exists = await dbContext.Departments
        .AnyAsync(d => d.Id != viewModel.Id && d.Name.ToLower() == viewModel.Name.ToLower());

            if (exists)
            {
                return Conflict(new { success = false, message = "A department with the same name already exists." });
            }

            department.Name = viewModel.Name;
            department.CreatedAt = DateTime.UtcNow;
            await dbContext.SaveChangesAsync();

            return Ok(new { success = true, message = "Department Updated Successfully" });
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] Guid id)
        {
            var department = await dbContext.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound(new { success = false, message = "Department not found!" });
            }

            dbContext.Departments.Remove(department);
            await dbContext.SaveChangesAsync();

            return Ok(new { success = true, message = "Department deleted successfully." });
        }

         
        [HttpPost]
        public async Task<IActionResult> DataTable()
        {
            var form = Request.Form;

            int draw = int.Parse(form["draw"]);
            int start = int.Parse(form["start"]);
            int length = int.Parse(form["length"]);

            string sortColumnIndex = form["order[0][column]"];
            string sortColumn = form[$"columns[{sortColumnIndex}][data]"];
            string sortDirection = form["order[0][dir]"];
            string searchValue = form["search[value]"];

            var query = dbContext.Departments.AsQueryable();

            // search
            if (!string.IsNullOrEmpty(searchValue))
            {
                searchValue = searchValue.ToLower();
                query = query.Where(d => d.Name.ToLower().Contains(searchValue)|| d.CreatedAt.ToString().Contains(searchValue));
            }

            int recordsTotal = await query.CountAsync();

            // sorting
            switch (sortColumn)
            {
                case "name":
                    query = sortDirection == "asc"
                        ? query.OrderBy(d => d.Name)
                        : query.OrderByDescending(d => d.Name);
                    break;
                case "createdAt":
                    query = sortDirection == "asc"
                        ? query.OrderBy(d => d.CreatedAt)
                        : query.OrderByDescending(d => d.CreatedAt);
                    break;
                default:
                    query = query.OrderBy(d => d.Name); // Default sort
                    break;
            }

            var data = await query
                .Skip(start)
                .Take(length)
                .Select(d => new
                {
                    d.Id,
                    d.Name,
                    d.CreatedAt
                })
                .ToListAsync();

            return Json(new
            {
                draw = draw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = data
            });
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ListJson()
        {
            var departments = dbContext.Departments
                .Select(d => new { d.Id, d.Name })
                .ToList();
            return Json(departments);
        }



    }
}

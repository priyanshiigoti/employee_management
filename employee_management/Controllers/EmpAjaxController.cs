using employee_management.Database;
using employee_management.Models;
using employee_management.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace employee_management.Controllers
{
    public class EmpAjaxController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmpAjaxController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DataTable()
        {
            try
            {
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
                var sortColumn = Request.Form[$"columns[{sortColumnIndex}][data]"].FirstOrDefault();
                var sortDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                int pageSize = string.IsNullOrEmpty(length) ? 10 : Convert.ToInt32(length);
                int skip = string.IsNullOrEmpty(start) ? 0 : Convert.ToInt32(start);

                var query = _context.Employees.Include(e => e.department).AsQueryable();

                if (!string.IsNullOrEmpty(searchValue))
                {
                    var lowerSearch = searchValue.ToLower();
                    query = query.Where(e =>
                        e.FirstName.ToLower().Contains(lowerSearch) ||
                        e.LastName.ToLower().Contains(lowerSearch) ||
                        e.Email.ToLower().Contains(lowerSearch) ||
                        e.Phone.ToLower().Contains(lowerSearch) ||
                        e.IsActive.ToString().ToLower().Contains(lowerSearch) ||
                        (e.department != null && e.department.Name.ToLower().Contains(lowerSearch))
                    );
                }

                if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortDirection))
                {
                    sortColumn = char.ToUpper(sortColumn[0]) + sortColumn.Substring(1);
                    query = query.OrderBy($"{sortColumn} {sortDirection}");
                }

                var recordsFiltered = await query.CountAsync();
                var data = await query.Skip(skip).Take(pageSize).Select(e => new
                {
                    e.Id,
                    e.FirstName,
                    e.LastName,
                    e.Email,
                    e.Phone,
                    e.IsActive,
                    DepartmentName = e.department != null ? e.department.Name : "",
                    e.DepartmentId
                }).ToListAsync();

                var totalRecords = await _context.Employees.CountAsync();

                return Json(new
                {
                    draw,
                    recordsTotal = totalRecords,
                    recordsFiltered,
                    data
                });
            }
            catch (Exception ex)
            {
                return Json(new { error = "Error loading data: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddorEditEmployee emp)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid data." });

            bool exists = await _context.Employees.AnyAsync(e =>
                e.Email.ToLower() == emp.Email.Trim().ToLower() ||
                e.Phone.Trim() == emp.Phone.Trim());

            if (exists)
            {
                return Conflict(new { success = false, message = "Employee with this email or phone number already exists." });
            }

            var employee = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = emp.FirstName,
                LastName = emp.LastName,
                Email = emp.Email,
                Phone = emp.Phone,
                DepartmentId = emp.DepartmentId,
                IsActive = emp.IsActive
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Employee added successfully!" });
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid id)
        {
            var emp = await _context.Employees.FindAsync(id);
            if (emp == null)
                return NotFound(new { success = false, message = "Employee not found" });

            var empViewModel = new AddorEditEmployee
            {
                Id = emp.Id,
                FirstName = emp.FirstName,
                LastName = emp.LastName,
                Email = emp.Email,
                Phone = emp.Phone,
                DepartmentId = emp.DepartmentId,
                IsActive = emp.IsActive
            };

            return Json(empViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] AddorEditEmployee emp)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid data." });

            var existing = await _context.Employees.FindAsync(emp.Id);
            if (existing == null)
                return NotFound(new { success = false, message = "Employee not found" });

            bool duplicate = await _context.Employees.AnyAsync(e =>
                e.Id != emp.Id &&
                (e.Email.ToLower() == emp.Email.Trim().ToLower() || e.Phone == emp.Phone.Trim()));

            if (duplicate)
            {
                return Conflict(new { success = false, message = "Another employee with the same email or phone exists." });
            }

            existing.FirstName = emp.FirstName;
            existing.LastName = emp.LastName;
            existing.Email = emp.Email;
            existing.Phone = emp.Phone;
            existing.DepartmentId = emp.DepartmentId;
            existing.IsActive = emp.IsActive;

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Employee updated successfully!" });
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] Guid id)
        {
            var emp = await _context.Employees.FindAsync(id);
            if (emp == null)
                return NotFound(new { success = false, message = "Employee not found" });

            _context.Employees.Remove(emp);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Employee deleted successfully!" });
        }
    }
}

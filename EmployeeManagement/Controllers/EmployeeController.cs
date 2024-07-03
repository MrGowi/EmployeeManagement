using EmployeeManagement.Data;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;

namespace EmployeeManagement.Controllers
{
    public class EmployeeController : Controller
    {

        private readonly ApplicationDbContext _dbContext;

        public EmployeeController(ApplicationDbContext Context)
        {
            _dbContext = Context;
        }

        public IActionResult Index()
        {
            var employees = _dbContext.Employees.ToList();

            ViewBag.Employees = employees;

            return View();
        }

        public IActionResult CreateEdit(int id)
        {
            if (id == 0)
            {
                return View("CreateEditEmployee");
            }

            var employeeInDb = _dbContext.Employees.Find(id);

            if (employeeInDb == null)
            {
                return NotFound();
            }

            return View("CreateEditEmployee", employeeInDb);
        }

        [HttpPost]
        public IActionResult CreateEditEmployee(Employee employee)
        {
            if (employee.Id == 0)
            {
                _dbContext.Employees.Add(employee);
            }
            else
            {
                _dbContext.Employees.Update(employee);
            }
            _dbContext.Employees.Add(employee);
            _dbContext.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult DeleteEmployee(int id)
        {
            var employeeInDb = _dbContext.Employees.Find(id);

            if (employeeInDb == null)
            {
                return NotFound();
            }
            else
            {
                _dbContext.Employees.Remove(employeeInDb);
                _dbContext.SaveChanges();
            }

            return RedirectToAction("Index");

        }

    }
}


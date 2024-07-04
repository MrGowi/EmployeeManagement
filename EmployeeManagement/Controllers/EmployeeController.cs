using EmployeeManagement.Data;
using EmployeeManagement.Models;
using EmployeeManagement.Services;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using OfficeOpenXml;
using System.Net;
using System.Net.Mail;

namespace EmployeeManagement.Controllers
{
    public class EmployeeController : Controller
    {

        private readonly ApplicationDbContext _dbContext;
        private readonly IEmailSender emailSender;


        public EmployeeController(ApplicationDbContext Context, IEmailSender emailSender)
        {
            _dbContext = Context;
            this.emailSender = emailSender;

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

        public IActionResult SendEmail(int id)
        {
            var employeeInDb = _dbContext.Employees.Find(id);

            if (employeeInDb == null)
            {
                return NotFound();
            }

            return View("SendEmail", employeeInDb);
        }

        
        [HttpPost]
        public async Task<IActionResult> SendEmail(int id, string subject, string body)
        {
            var employeeInDb = _dbContext.Employees.Find(id);
            if (employeeInDb == null)
            {
                return NotFound();
            }

            try
            {
                await emailSender.SendEmailAsync(employeeInDb.Email, subject, body);
                ViewBag.Message = "Email sent successfully!";
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Error sending email: {ex.Message}";
            }

            return View(employeeInDb);
        }


        public IActionResult ExportTable()
        {
            var employees = _dbContext.Employees.ToList();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Employees");
                worksheet.Cells["A1"].Value = "ID";
                worksheet.Cells["B1"].Value = "Vorname";
                worksheet.Cells["C1"].Value = "Nachname";

                int row = 2;
                foreach (var employee in employees)
                {
                    worksheet.Cells[row, 1].Value = employee.Id;
                    worksheet.Cells[row, 2].Value = employee.Firstname;
                    worksheet.Cells[row, 3].Value = employee.Lastname;
                    row++;
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string excelName = $"Employees-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }


    }

}



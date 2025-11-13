using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NoSQL_Project.Enums;
using NoSQL_Project.Models;
using NoSQL_Project.Services;
using NoSQL_Project.ViewModels;
using System.Data;


namespace NoSQL_Project.Controllers
{
    public class EmployeesController : BaseController
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService) => _employeeService = employeeService;

        public async Task<IActionResult> Index(Location? location, UserRole? userRole)
        {
            List<Employee> employees = await _employeeService.GetAllAsync(location, userRole);
            EmployeeViewModel employeeViewModel = new EmployeeViewModel
            {
                EmployeesList = employees,
                SelectedLocation = location,
                SelectedUserRole = userRole,
            };
            return View(employeeViewModel);
        }

        // Hamza's Code

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (!ModelState.IsValid)
                return View(loginModel);

            // Check credentials (service handles hashing)
            var employee = await _employeeService.GetByLoginCredentialAsync(loginModel.Email, loginModel.Password);

            if (employee == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(loginModel);
            }

            // ✅ Store in session object id class is available but just implemented later
            HttpContext.Session.SetObject("LoggedInUser", employee);

            switch (employee.UserRole.ToString().ToLower())
            {
                case "employee":
                    return RedirectToAction("Index", "Tickets");
                case "service_desk_employee":
                    return RedirectToAction("DashBoard", "Tickets");
                case "manager":
                    return RedirectToAction("DashBoard", "Tickets");
                default:
                    return RedirectToAction("Login", "Employees");

            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Employees");
        }

        public IActionResult CheckSession()
        {
            var id = HttpContext.Session.GetString("EmployeeId");
            var name = HttpContext.Session.GetString("EmployeeName");
            var role = HttpContext.Session.GetString("EmployeeRole");

            return Content($"ID: {id ?? "none"} | Name: {name ?? "none"} | Role: {role ?? "none"}");
        }

        // David's Code

        [HttpGet]
        public IActionResult AddEmployee()
        {
            var viewModel = new EmployeeViewModel
            {
                Employee = new Employee(),
                UserRoleOptions = Enum.GetValues(typeof(UserRole))
                    .Cast<UserRole>()
                    .Select(r => new SelectListItem { Text = r.ToString(), Value = r.ToString() }),
                LocationOptions = Enum.GetValues(typeof(Location))
                    .Cast<Location>()
                    .Select(l => new SelectListItem { Text = l.ToString(), Value = l.ToString() })

            };
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEmployee(Employee employee) // Changed to async Task<IActionResult>
        {
            var viewModel = new EmployeeViewModel
            {
                Employee = employee,
                UserRoleOptions = Enum.GetValues(typeof(UserRole))
                    .Cast<UserRole>()
                    .Select(r => new SelectListItem { Text = r.ToString(), Value = r.ToString() }),
                LocationOptions = Enum.GetValues(typeof(Location))
                    .Cast<Location>()
                    .Select(l => new SelectListItem { Text = l.ToString(), Value = l.ToString() })
            };

            try
            {
                await _employeeService.AddEmployeeAsync(employee); // Added await
                TempData["SuccessMessage"] = "Employee has been added successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Exception occurred: {ex.Message}";
                return View(viewModel);
            }
        }

        [HttpGet]
        public IActionResult UpdateEmployee(string id)
        {
            var employee = _employeeService.GetByIdAsync(id).Result; // Synchronously wait for the result
            if (employee == null)
            {
                return NotFound();
            }
            var viewModel = new EmployeeViewModel
            {
                Employee = employee,
                UserRoleOptions = Enum.GetValues(typeof(UserRole))
                    .Cast<UserRole>()
                    .Select(r => new SelectListItem { Text = r.ToString(), Value = r.ToString() }),
                LocationOptions = Enum.GetValues(typeof(Location))
                    .Cast<Location>()
                    .Select(l => new SelectListItem { Text = l.ToString(), Value = l.ToString() })
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateEmployee(Employee employee)
        {
            var viewModel = new EmployeeViewModel
            {
                Employee = employee,
                UserRoleOptions = Enum.GetValues(typeof(UserRole))
                    .Cast<UserRole>()
                    .Select(r => new SelectListItem { Text = r.ToString(), Value = r.ToString() }),
                LocationOptions = Enum.GetValues(typeof(Location))
                    .Cast<Location>()
                    .Select(l => new SelectListItem { Text = l.ToString(), Value = l.ToString() })
            };

            try
            {
                var existingEmployee = await _employeeService.GetByIdAsync(employee.EmployeeId);
                if (existingEmployee == null)
                {
                    ViewBag.ErrorMessage = "Employee not found.";
                    return View(viewModel);
                }

                await _employeeService.UpdateEmployeeAsync(employee.EmployeeId, employee);

                TempData["SuccessMessage"] = "Employee has been updated successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Exception occurred: {ex.Message}";
                return View(viewModel);
            }
        }

        [HttpGet]
        public IActionResult SoftDeleteEmployee(string id)
        {
            var employee = _employeeService.GetByIdAsync(id).Result;
            if (employee == null)
            {
                return NotFound();
            }
            var viewModel = new EmployeeViewModel
            {
                Employee = employee,
            };
            return View(viewModel);
        }
        [HttpPost, ActionName("SoftDeleteEmployee")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDeleteEmployeeConfirmed(string id)
        {
			try
			{
				var employee = await _employeeService.GetByIdAsync(id);
				if (employee == null)
				{
					TempData["ErrorMessage"] = "Employee not found.";
					return RedirectToAction("Index");
				}

				// Set IsActive to false
				employee.IsActive = false;

				// Use your UpdateEmployeeAsync method
				await _employeeService.UpdateEmployeeAsync(id, employee);

				TempData["SuccessMessage"] = "Employee has been deactivated successfully";
				return RedirectToAction("Index");
			}
			catch (Exception ex)
			{
				ViewBag.ErrorMessage = $"Exception occurred: {ex.Message}";
				var employee = await _employeeService.GetByIdAsync(id);
				var viewModel = new EmployeeViewModel
				{
					Employee = employee
				};
				return View(viewModel);
			}
		}        
    }
}

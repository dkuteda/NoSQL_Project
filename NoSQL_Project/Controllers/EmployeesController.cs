using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NoSQL_Project.Models;
using NoSQL_Project.Repositories.Interfaces;
using System.Data;
using NoSQL_Project.Enums;
using NoSQL_Project.ViewModels;
using NoSQL_Project.Repositories;
using NoSQL_Project.Services;
using Microsoft.AspNetCore.Http;


namespace NoSQL_Project.Controllers
{
	public class EmployeesController : Controller
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
			HttpContext.Session.SetString("EmployeeId", employee.EmployeeId);
			HttpContext.Session.SetString("EmployeeName", employee.FirstName);
			HttpContext.Session.SetString("EmployeeRole", employee.UserRole.ToString());

			switch (employee.UserRole.ToString().ToLower())
			{
				case "employee":
					return RedirectToAction("Index", "Tickets"); 
				case "service_desk_employee ":
					return RedirectToAction("Index", "Employees"); 
				case "manager":
					return RedirectToAction("Index", "Employees"); 
				default:
					return RedirectToAction("Index", "Home"); 

			}         

        }

		public IActionResult Logout()
		{
			HttpContext.Session.Clear();
			return RedirectToAction("Login", "Employees");
		}

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

				if (!string.IsNullOrWhiteSpace(employee.FirstName))
					existingEmployee.FirstName = employee.FirstName;

				if (!string.IsNullOrWhiteSpace(employee.LastName))
					existingEmployee.LastName = employee.LastName;

				if (!string.IsNullOrWhiteSpace(employee.Email))
					existingEmployee.Email = employee.Email.Trim();

				if (!string.IsNullOrWhiteSpace(employee.PhoneNumber))
					existingEmployee.PhoneNumber = employee.PhoneNumber;

				if (employee.Gender.HasValue && employee.Gender.Value != default(Gender))
					existingEmployee.Gender = employee.Gender.Value;

				if (employee.Location.HasValue && employee.Location.Value != default(Location))
					existingEmployee.Location = employee.Location.Value;

				if (employee.UserRole.HasValue && employee.UserRole.Value != default(UserRole))
					existingEmployee.UserRole = employee.UserRole.Value;

				if (!string.IsNullOrWhiteSpace(employee.Password))
				{
					existingEmployee.Password = employee.Password; // HashPassword(employee.Password);
				}
				existingEmployee.IsActive = employee.IsActive;

				await _employeeService.UpdateEmployeeAsync(existingEmployee);

				TempData["SuccessMessage"] = "Employee has been updated successfully";
				return RedirectToAction("Index");
			}
			catch (Exception ex)
			{
				ViewBag.ErrorMessage = $"Exception occurred: {ex.Message}";
				return View(viewModel);
			}
		}
		/*private string HashPassword(string password)
		{
			return password;
		}*/

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
				bool isDeleted = await _employeeService.SoftDeleteAsync(id);
				if (isDeleted)
				{
					TempData["SuccessMessage"] = "Employee has been deactivated successfully";
				}
				else
				{
					TempData["ErrorMessage"] = "Employee not found or already inactive";
				}
				return RedirectToAction("Index");
			}
			catch (Exception ex)
			{
				ViewBag.ErrorMessage = $"Exception occurred: {ex.Message}";
				var employee = await _employeeService.GetByIdAsync(id);
				var viewModel = new EmployeeViewModel
				{
					Employee = employee,
				};
				return View(viewModel);
			}
		}
        public IActionResult CheckSession()
        {
            var id = HttpContext.Session.GetString("EmployeeId");
            var name = HttpContext.Session.GetString("EmployeeName");
            var role = HttpContext.Session.GetString("EmployeeRole");

            return Content($"ID: {id ?? "none"} | Name: {name ?? "none"} | Role: {role ?? "none"}");
        }
    }
}

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

		public async Task<IActionResult> Index()
		{
			List<Employee> employees = await _employeeService.GellAsync();
			EmployeeViewModel employeeViewModel = new EmployeeViewModel
			{
				EmployeesList = employees
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
			var employee = await _employeeService.GetByLoginCredentialAsync(loginModel.FirstName, loginModel.Password);

			if (employee == null)
			{
				ModelState.AddModelError(string.Empty, "Invalid email or password.");
				return View(loginModel);
			}


			// ✅ Store in session
			HttpContext.Session.SetString("EmployeeId", employee.EmployeeId);
			HttpContext.Session.SetString("EmployeeName", employee.FirstName);

            switch (employee.UserRole.ToString().ToLower())
            {
                case "manager":
                    return RedirectToAction("Dashboard", "Manager");
                case "servicedesk":
                    return RedirectToAction("Index", "Tickets");
                default:
                    return RedirectToAction("Create", "Tickets");
            }


           
		}

		public IActionResult Logout()
		{
			HttpContext.Session.Clear();
			return RedirectToAction("Login");
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
				GenderOptions = Enum.GetValues(typeof(Gender))
					.Cast<Gender>()
					.Select(g => new SelectListItem { Text = g.ToString(), Value = g.ToString() }),
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
				GenderOptions = Enum.GetValues(typeof(Gender))
					.Cast<Gender>()
					.Select(g => new SelectListItem { Text = g.ToString(), Value = g.ToString() }),
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
				GenderOptions = Enum.GetValues(typeof(Gender))
					.Cast<Gender>()
					.Select(g => new SelectListItem { Text = g.ToString(), Value = g.ToString() }),
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
				GenderOptions = Enum.GetValues(typeof(Gender))
				.Cast<Gender>()
				.Select(g => new SelectListItem { Text = g.ToString(), Value = g.ToString() }),
				LocationOptions = Enum.GetValues(typeof(Location))
					.Cast<Location>()
					.Select(l => new SelectListItem { Text = l.ToString(), Value = l.ToString() })
			};
			try
			{
				await _employeeService.UpdateEmployeeAsync(employee); // Added await
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
	}
}

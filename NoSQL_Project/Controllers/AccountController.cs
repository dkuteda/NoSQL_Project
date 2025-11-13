using Microsoft.AspNetCore.Mvc;
using NoSQL_Project.Models;
using NoSQL_Project.Services;
using NoSQL_Project.ViewModels;

namespace NoSQL_Project.Controllers
{
	public class AccountController : Controller
	{
		private readonly IEmployeeService _employeeService;

		public AccountController(IEmployeeService employeeService)
			=> _employeeService = employeeService;

		[HttpGet]
		public async Task<IActionResult> ForgotPassword(string email)
		{
			if (string.IsNullOrEmpty(email))
			{
				return View();
			}

			var employee = await _employeeService.GetByEmailAsync(email);
			if (employee == null)
			{
				return View();
			}

			return RedirectToAction("ResetPassword", new { email = employee.Email });
		}

		[HttpGet]
		public async Task<IActionResult> ResetPassword(string email)
		{
			var employee = await _employeeService.GetByEmailAsync(email);
			if (employee == null)
			{
				return RedirectToAction("ForgotPassword");
			}

			var viewModel = new EmployeeViewModel
			{
				Employee = employee
			};

			return View(viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ResetPassword(Employee employee)
		{
			var viewModel = new EmployeeViewModel
			{
				Employee = employee
			};

			try
			{
				var existingEmployee = await _employeeService.GetByEmailAsync(employee.Email);
				if (existingEmployee == null)
				{
					return RedirectToAction("ForgotPassword");
				}

				await _employeeService.UpdateEmployeeAsync(employee.EmployeeId, employee);

				return RedirectToAction("Login", "Employees");
			}
			catch (Exception ex)
			{
				ViewBag.ErrorMessage = $"Exception occurred: {ex.Message}";
				return View(viewModel);
			}
		}
	}
}

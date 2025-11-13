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

		// Step 1: ForgotPassword GET
		[HttpGet]
		public async Task<IActionResult> ForgotPassword(string email)
		{
			if (string.IsNullOrEmpty(email))
			{
				// Just show the page with the email input
				return View();
			}

			var employee = await _employeeService.GetByEmailAsync(email);
			if (employee == null)
			{
				// Employee not found → stay on ForgotPassword page
				return View();
			}

			// Employee found → redirect to ResetPassword GET
			return RedirectToAction("ResetPassword", new { email = employee.Email });
		}

		// Step 2: ResetPassword GET (shows the form)
		[HttpGet]
		public async Task<IActionResult> ResetPassword(string email)
		{
			var employee = await _employeeService.GetByEmailAsync(email);
			if (employee == null)
			{
				// If email not found, go back to ForgotPassword
				return RedirectToAction("ForgotPassword");
			}

			var viewModel = new EmployeeViewModel
			{
				Employee = employee
			};

			return View(viewModel);
		}

		// Step 3: ResetPassword POST (handles form submission)
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
					// If not found, go back to ForgotPassword
					return RedirectToAction("ForgotPassword");
				}

				// Update employee with new password
				await _employeeService.UpdateEmployeeAsync(employee.EmployeeId, employee);

				// After reset, redirect to ForgotPassword (since no Index exists)
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

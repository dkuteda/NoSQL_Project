using Microsoft.AspNetCore.Mvc;
using NoSQL_Project.Services;
using NoSQL_Project.Services.Interfaces;
using NoSQL_Project.ViewModels;

namespace NoSQL_Project.Controllers
{
	public class AccountController : Controller
	{
		private readonly IEmployeeService _employeeService;

		public AccountController(IEmployeeService employeeService)
		{
			_employeeService = employeeService;
		}

		[HttpGet]
		public IActionResult ForgotPassword()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
		{
			if (!ModelState.IsValid)
				return View(model);
			await _employeeService.SendPasswordResetLinkAsync(model.Email);
			// Always show confirmation view (don’t reveal if email exists)
			return View("ForgotPasswordConfirmation");
		}

		[HttpGet]
		public IActionResult ResetPassword(string email, string token)
		{
			if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
				return BadRequest("Invalid password reset request.");
			return View(new ResetPasswordViewModel { Email = email, Token = token });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
		{
			if (!ModelState.IsValid)
				return View(model);
			var result = await _employeeService.ResetPasswordAsync(model);
			if (result.Succeeded)
				return View("ResetPasswordConfirmation");
			foreach (var error in result.Errors)
				ModelState.AddModelError("", error.Description);
			return View(model);
		}
	}
}

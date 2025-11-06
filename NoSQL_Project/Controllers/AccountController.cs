using Microsoft.AspNetCore.Mvc;
using NoSQL_Project.ViewModels;

namespace NoSQL_Project.Controllers
{
	public class AccountController : Controller
	{
		public async Task<IActionResult> ForgotPassword()
		{
			return View();
		}

		[HttpPost]
		public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model) 
		{
			return View();
		}

		public async Task<ActionResult> ForgotPasswordConfirmation()
	}
}

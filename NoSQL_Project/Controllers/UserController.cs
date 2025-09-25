using Microsoft.AspNetCore.Mvc;
using NoSQL_Project.Models;
using NoSQL_Project.Repositories.Interfaces;

namespace NoSQL_Project.Controllers
{
	public class UserController : Controller
	{
		private readonly IUserRepository _repo;

		public UserController(IUserRepository repo) => _repo = repo;

		/*public IActionResult Login(LoginModel loginModel)
		{
			if (ModelState.IsValid)
			{
				var user = _repo.GetByLoginCredentials(loginModel.Email, loginModel.Password);
				if (user == null)
				{
					ViewBag.ErrorMessage = "Incorrect combination of username and password.";
					return View(loginModel);
				}
				else
				{

					
				}

			}

		}*/
		public IActionResult Index()
		{
			var users = _repo.GetAll();
			return View(users);
		}

		[HttpPost]
		public IActionResult Create()
		{
			return RedirectToAction("Index");
		}
	}
}

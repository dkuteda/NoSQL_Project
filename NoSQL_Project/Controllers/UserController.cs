using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NoSQL_Project.Models;
using NoSQL_Project.Repositories.Interfaces;
using System.Data;
using NoSQL_Project.Enums;
using NoSQL_Project.ViewModels;
using NoSQL_Project.Repositories;

namespace NoSQL_Project.Controllers
{
	public class UserController : Controller
	{
		private readonly IEmployeeRepository _repo;

		public UserController(IEmployeeRepository repo) => _repo = repo;

		public IActionResult Index()
		{
			var employees = _repo.GellAsync();
			return View(employees);
		}
		[HttpGet]
		public IActionResult AddEmployee()
		{
			var viewModel = new EmployeeViewModel
			{
				Employee = new Employees(),
				UserRoleOptions = Enum.GetValues(typeof(UserRole))
					.Cast<UserRole>()
					.Select(r => new SelectListItem { Text = r.ToString(), Value = r.ToString() })
				GenderOptions = Enum.GetValues(typeof(Gender))
					.Cast<Gender>()
					.Select(g => new SelectListItem { Text = g.ToString(), Value = g.ToString() }),
				LocationOptions = Enum.GetValues(typeof(Location))
					.Cast<Location>()
					.Select(l => new SelectListItem { Text = l.ToString(), Value = l.ToString() })

			};
			return View(viewModel);
		}
	}
}

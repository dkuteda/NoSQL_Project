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
	public class EmployeesController : Controller
	{
		private readonly IEmployeeRepository _employeeRepo;

		public EmployeesController(IEmployeeRepository repo) => _employeeRepo = repo;

		public async Task<IActionResult> Index()
		{
			List<Employees> employees = await _employeeRepo.GellAsync();
			EmployeeViewModel employeeViewModel = new EmployeeViewModel
			{
				EmployeesList = employees
			};

			return View(employeeViewModel);
		}
		[HttpGet]
		public IActionResult AddEmployee()
		{
			var viewModel = new EmployeeViewModel
			{
				Employee = new Employees(),
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
		public async Task<IActionResult> AddEmployee(Employees employee) // Changed to async Task<IActionResult>
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
				await _employeeRepo.AddEmployeeAsync(employee); // Added await
				TempData["SuccessMessage"] = "Employee has been added successfully";
				return RedirectToAction("Index");
			}
			catch (Exception ex)
			{
				ViewBag.ErrorMessage = $"Exception occurred: {ex.Message}";
				return View(viewModel);
			}
		}
	}
}

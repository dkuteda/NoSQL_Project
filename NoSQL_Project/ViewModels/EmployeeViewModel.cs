using NoSQL_Project.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using NoSQL_Project.Enums;

namespace NoSQL_Project.ViewModels
{
	public class EmployeeViewModel
	{
		public Employees Employee { get; set; } 
		public IEnumerable<SelectListItem> UserRoleOptions { get; set; }
		public IEnumerable<SelectListItem> GenderOptions { get; set; }
		public IEnumerable<SelectListItem> LocationOptions { get; set; }
	}
}

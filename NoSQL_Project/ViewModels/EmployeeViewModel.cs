using NoSQL_Project.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using NoSQL_Project.Enums;

namespace NoSQL_Project.ViewModels
{
	public class EmployeeViewModel
	{
		public Employee Employee { get; set; } 

		// Used for Adding (so far) 
		public IEnumerable<SelectListItem> UserRoleOptions { get; set; }
		public IEnumerable<SelectListItem> GenderOptions { get; set; }
		public IEnumerable<SelectListItem> LocationOptions { get; set; }

		// Used for index
		public List<Employee> EmployeesList { get; set; }
	}
}

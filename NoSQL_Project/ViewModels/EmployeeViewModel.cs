using Microsoft.AspNetCore.Mvc.Rendering;
using NoSQL_Project.Enums;
using NoSQL_Project.Models;
using System.Data;

namespace NoSQL_Project.ViewModels
{
	public class EmployeeViewModel
	{
		public Employee Employee { get; set; } 

		// Used for Adding...
		public IEnumerable<SelectListItem> UserRoleOptions { get; set; }
		public IEnumerable<SelectListItem> LocationOptions { get; set; }

		// Used for index
		public List<Employee> EmployeesList { get; set; }
		public UserRole? SelectedUserRole { get; set; }
		public Location? SelectedLocation { get; set; }


		public UserRole[] UserRoles { get; set; }
		public Location[] Locations { get; set; }

		public EmployeeViewModel() 
		{
			UserRoles = Enum.GetValues(typeof(UserRole)) as UserRole[] ?? new UserRole[0];
			Locations = Enum.GetValues(typeof(Location)) as Location[] ?? new Location[0];
		}
	}
}

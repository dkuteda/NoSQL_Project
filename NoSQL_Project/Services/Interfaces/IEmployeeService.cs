using NoSQL_Project.Enums;	
using NoSQL_Project.Models;
using NoSQL_Project.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace NoSQL_Project.Services
{
	public interface IEmployeeService
	{
		Task<List<Employee>> GetAllAsync(Location? location, UserRole? userRole);
		Task<Employee> GetByIdAsync(string id);
		Task AddEmployeeAsync(Employee employees);
		Task UpdateEmployeeAsync(string id, Employee employees);
		Task<bool> SoftDeleteAsync(string id);
		Task<Employee?> GetByLoginCredentialAsync(string email, string password);
		Task<bool> EmailAddressExistsAsync(string email);
		Task<List<Employee>> AutocompleteSearchEmployees(string name);


		//New Methods for Forgot Password
		Task<bool> SendPasswordResetLinkAsync(string email);
		Task<IdentityResult> ResetPasswordAsync(ResetPasswordViewModel model);
	}
}

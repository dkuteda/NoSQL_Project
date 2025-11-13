using NoSQL_Project.Enums;
using NoSQL_Project.Models;
using NoSQL_Project.Repositories.Interfaces;
using System.Security.Cryptography;
using System.Text;


namespace NoSQL_Project.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepo;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepo = employeeRepository;
        }

        public async Task<List<Employee>> GetAllAsync(Location? location, UserRole? userRole)
        {
            return await _employeeRepo.GetAllAsync(location, userRole);
        }

        public async Task<Employee> GetByIdAsync(string id)
        {
            return await _employeeRepo.GetByIdAsync(id);
        }

        public async Task AddEmployeeAsync(Employee employees)
        {
            if (EmailAddressExistsAsync(employees.Email).Result)
                throw new Exception("Email address already in use.");

            employees.Password = HashPassword(employees.Password);
            await _employeeRepo.AddEmployeeAsync(employees);
        }
        public async Task<Employee> GetByEmailAsync(string email) 
        { 
            return await _employeeRepo.GetByEmailAsync(email);
		}

		public async Task UpdateEmployeeAsync(string id, Employee employee)
        {
            if (!string.IsNullOrWhiteSpace(employee.Password))
            {
                employee.Password = HashPassword(employee.Password);
                await _employeeRepo.UpdateEmployeeAsync(id, employee);
            }
            else
            {
                var existingEmployee = await _employeeRepo.GetByIdAsync(id);
                if (existingEmployee != null)
                {
                    employee.Password = existingEmployee.Password;
                    await _employeeRepo.UpdateEmployeeAsync(id, employee);
                }
            }
        }

        public async Task<bool> EmailAddressExistsAsync(string email)
        {
            return await _employeeRepo.EmailAddressExistsAsync(email);
        }


        //Hamza' Code
        public async Task<Employee?> GetByLoginCredentialAsync(string email, string password)
        {
            // hash entered password before checking
            var hashed = HashPassword(password);
            return await _employeeRepo.GetByLoginCredentialAsync(email, hashed);
        }
        private static string HashPassword(string password)
        {
            // use B authentication
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public async Task<List<Employee>> AutocompleteSearchEmployees(string name)
        {
            return await _employeeRepo.AutocompleteSearchEmployees(name);
        }
    }
}


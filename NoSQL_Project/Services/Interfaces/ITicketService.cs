using NoSQL_Project.Models;
using NoSQL_Project.ViewModels;

namespace NoSQL_Project.Services.Interfaces
{
    public interface ITicketService
    {
        Task UpdateTicketAsync(Ticket ticket);

        Task<Ticket> GetByIdAsync(string id);

        TicketViewModel FillTicketInfo(Ticket ticket);

        Task<List<Ticket>> GetAllTicketsAsync();
        Task<List<Ticket>> GetTicketsByEmployeeIdAsync(EmployeeDetails employee);

        Task CreateTicketAsync(Ticket ticket);

        Task<bool> CloseAsync(Ticket ticket);

        Task<(int total, int resolved, int transferred)> GetEmployeeStatsAsync(string firstName, string lastName);
    }
}

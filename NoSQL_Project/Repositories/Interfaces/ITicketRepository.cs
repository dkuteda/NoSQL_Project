using NoSQL_Project.Models;
using NoSQL_Project.ViewModels;

namespace NoSQL_Project.Repositories.Interfaces
{
    public interface ITicketRepository
    {
        //Nana Yaa's Methods
        Task<List<Ticket>> GetAllTicketsAsync();
        Task<List<Ticket>> GetTicketsByEmployeeIdAsync(Employee employee);
        Task CreateTicketAsync(Ticket ticket);
        Task AddResolutionStep(string ticketId, EmployeeDetails details);
        Task AssignMyselfToTicket(Ticket ticket, EmployeeDetails details);

        //Thijmen's methods
        Task<Ticket> GetByIdAsync(string id);
        Task UpdateTicketAsync(Ticket ticket);
        Task UpdateEscalation(EscalateViewModel escalationTicket);
        Task<bool> CloseAsync(Ticket ticket);

        //Hamza's methods
        Task<List<Ticket>> SearchTicketsAsync(string searchText, bool useAnd);
        Task<(int total, int open, int resolved, int closed)> GetDashboardStatsAsync(string? employeeId);

    }
}

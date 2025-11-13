using NoSQL_Project.Models;
using NoSQL_Project.ViewModels;

namespace NoSQL_Project.Services.Interfaces
{
    public interface ITicketService
    {
        //Nana Yaa's code
        Task<List<Ticket>> GetAllTicketsAsync();
        Task<List<Ticket>> GetTicketsByEmployeeIdAsync(Employee employee);
        Task CreateTicketAsync(Ticket ticket);
        Task AddResolutionStep(string ticketId, EmployeeDetails details);
        Task AssignMyselfToTicket(Ticket ticket, EmployeeDetails details);

        //Thijmen's code
        Task<Ticket> GetByIdAsync(string id);
        Task UpdateTicketAsync(Ticket ticket);
        Task<bool> CloseAsync(Ticket ticket);
        EscalateViewModel FillEscalateInfo(Ticket ticket);
        Task UpdateEscalation(EscalateViewModel escalationTicket);

        //Hamza's code
        Task<(int total, int open, int resolved, int closed)> GetDashboardStatsAsync(string? employeeId);
        Task<List<Ticket>> SearchTicketsAsync(string searchText, bool useAnd);

    }
}

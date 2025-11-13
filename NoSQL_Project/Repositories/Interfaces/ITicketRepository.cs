using NoSQL_Project.Models;
using NoSQL_Project.ViewModels;

namespace NoSQL_Project.Repositories.Interfaces
{
    public interface ITicketRepository
    {
        Task UpdateTicketAsync(Ticket ticket);

        Task<Ticket> GetByIdAsync(string id);

        Task<List<Ticket>> GetAllTicketsAsync();
        Task<List<Ticket>> GetTicketsByEmployeeIdAsync(Employee employee);

        Task CreateTicketAsync(Ticket ticket);

        Task<bool> CloseAsync(Ticket ticket);

        Task<(int total, int resolved, int transferred)> GetEmployeeStatsAsync(string firstName, string lastName);

        Task<List<Ticket>> SearchTicketsAsync(string searchText, bool useAnd);
        Task AddResolutionStep(string ticketId, EmployeeDetails details);
        Task AssignMyselfToTicket(Ticket ticket, EmployeeDetails details);

        EscalateViewModel FillEscalateInfo(Ticket ticket);
        Task UpdateEscalation(EscalateViewModel escalationTicket);

    }
}

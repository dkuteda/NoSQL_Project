using NoSQL_Project.Enums;
using NoSQL_Project.Models;
using NoSQL_Project.Repositories.Interfaces;
using NoSQL_Project.Services.Interfaces;
using NoSQL_Project.ViewModels;

namespace NoSQL_Project.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepo;

        public TicketService(ITicketRepository ticketRepository)
        {
            _ticketRepo = ticketRepository;
        }

        //Nana Yaa's code
        public async Task<List<Ticket>> GetAllTicketsAsync()
        {
            return await _ticketRepo.GetAllTicketsAsync();
        }
        public async Task<List<Ticket>> GetTicketsByEmployeeIdAsync(Employee employee)
        {
            var result = await _ticketRepo.GetTicketsByEmployeeIdAsync(employee);
            return result ?? new List<Ticket>();
        }
        public async Task CreateTicketAsync(Ticket ticket)
        {
            await _ticketRepo.CreateTicketAsync(ticket);
        }
        public Task AddResolutionStep(string ticketId, EmployeeDetails details)
        {
            return _ticketRepo.AddResolutionStep(ticketId, details);
        }
        public Task AssignMyselfToTicket(Ticket ticket, EmployeeDetails details)
        {
            if (ticket.ResolutionSteps == null || !ticket.ResolutionSteps.Any())
            {
                return _ticketRepo.AssignMyselfToTicket(ticket, details);
            }
            throw new InvalidOperationException("This ticket is assigned to someone else/you're already assigned to it.");
        }

        //Thijmen's code
        public async Task<Ticket> GetByIdAsync(string id)
        {
            return await _ticketRepo.GetByIdAsync(id);
        }
        public async Task UpdateTicketAsync(Ticket ticket)
        {
            await _ticketRepo.UpdateTicketAsync(ticket);
        }
        public Task<bool> CloseAsync(Ticket ticket)
        {
            return _ticketRepo.CloseAsync(ticket);
        }
        public EscalateViewModel FillEscalateInfo(Ticket ticket)
        {
            Priority newPriority;
            DateTime now = DateTime.Now;
            DateTime newDeadline;

            if (ticket.Priority == Priority.low)
            {
                newPriority = Priority.normal;
                TimeSpan time = new TimeSpan(2, 0, 0, 0);
                newDeadline = now.Add(time);
            }
            else if (ticket.Priority == Priority.normal)
            {
                newPriority = Priority.high;
                TimeSpan time = new TimeSpan(1, 0, 0, 0);
                newDeadline = now.Add(time);
            }
            else
            {
                newPriority = Priority.high;
                newDeadline = ticket.Deadline;
            }
            return new EscalateViewModel(ticket, newPriority, newDeadline);
        }
        public Task UpdateEscalation(EscalateViewModel escalationTicket)
        {
            return _ticketRepo.UpdateEscalation(escalationTicket);
        }

        //Hamza's code
        public Task<(int total, int open, int resolved, int closed)> GetDashboardStatsAsync(string? employeeId)
        {
            return _ticketRepo.GetDashboardStatsAsync(employeeId);
        }
        public Task<List<Ticket>> SearchTicketsAsync(string searchText, bool useAnd)
        {
            return _ticketRepo.SearchTicketsAsync(searchText, useAnd);
        }
    }
}

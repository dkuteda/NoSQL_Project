using MongoDB.Driver;
using NoSQL_Project.Enums;
using NoSQL_Project.Models;
using NoSQL_Project.Repositories.Interfaces;
using NoSQL_Project.ViewModels;

namespace NoSQL_Project.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly IMongoCollection<Ticket> _tickets;

        public TicketRepository(IMongoDatabase db)
        {
            _tickets = db.GetCollection<Ticket>("tickets");
        }

        //Nana Yaa's code
        public async Task<List<Ticket>> GetAllTicketsAsync()
        {
            return await _tickets
                .Find(t => t.Status == TicketStatus.open)
                .SortByDescending(t => t.CreatedAt)
                .SortByDescending(t => t.Status)  // open first
                .ThenBy(t => t.Priority)          // higher priority first
                .ToListAsync();
        }
        public async Task<List<Ticket>> GetTicketsByEmployeeIdAsync(Employee employee)
        {
            return await _tickets
                .Find(t => t.CreatedBy.EmployeeId == employee.EmployeeId && t.Status == TicketStatus.open)
                .SortByDescending(t => t.CreatedAt)
                .ThenBy(t => t.Status)
                .ToListAsync();
        }
        public async Task CreateTicketAsync(Ticket ticket)
        {
            await _tickets.InsertOneAsync(ticket);
        }
        public async Task AddResolutionStep(string ticketId, EmployeeDetails details)
        {
            ResolutionStep step = new ResolutionStep(details, "Ticket transferred");
            var filter = Builders<Ticket>.Filter.Eq(t => t.TicketId, ticketId);
            var update = Builders<Ticket>.Update.Push(t => t.ResolutionSteps, step);

            var result = await _tickets.UpdateOneAsync(filter, update);

            if (!result.IsAcknowledged)
            {
                throw new InvalidOperationException("Write not acknowledged by MongoDB.");
            }
            else { Console.WriteLine("Update successful"); }
        }
        public async Task AssignMyselfToTicket(Ticket ticket, EmployeeDetails details)
        {
            ResolutionStep step = new ResolutionStep(details, "Assigned to self");
            var filter = Builders<Ticket>.Filter.Eq(t => t.TicketId, ticket.TicketId);
            var update = Builders<Ticket>.Update
                .Set(t => t.ResolutionSteps[0], step);
            var result = await _tickets.UpdateOneAsync(filter, update);
            if (!result.IsAcknowledged)
            {
                throw new InvalidOperationException("Write not acknowledged by MongoDB.");
            }
            else { Console.WriteLine("Update successful"); }
        }

        // Thijmen's code
        public async Task<Ticket> GetByIdAsync(string id)
        {
            return await _tickets.Find(s => s.TicketId == id).FirstOrDefaultAsync();
        }
        public async Task UpdateTicketAsync(Ticket ticket)
        {
            var update = Builders<Ticket>.Update
                                         .Set(t => t.Status, ticket.Status)
                                         .Set(t => t.Title, ticket.Title)
                                         .Set(t => t.Description, ticket.Description);

            await _tickets.UpdateOneAsync(t => t.TicketId == ticket.TicketId, update);
        }
        public async Task<bool> CloseAsync(Ticket ticket)
        {
            var filter = Builders<Ticket>.Filter.Eq(e => e.TicketId, ticket.TicketId);
            var update = Builders<Ticket>.Update.Set(e => e.Status, TicketStatus.closed);

            var result = await _tickets.UpdateOneAsync(filter, update);
            return result.IsAcknowledged && result.ModifiedCount > 0;
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

        public async Task UpdateEscalation(EscalateViewModel escalationTicket)
        {
            var update = Builders<Ticket>.Update
                            .Set(t => t.Priority, escalationTicket.NewPriority)
                            .Set(t => t.Deadline, escalationTicket.NewDeadline);

            await _tickets.UpdateOneAsync(t => t.TicketId == escalationTicket.Ticket.TicketId, update);
        }

        //Hamza's code
        public async Task<(int total, int open, int resolved, int closed)>
        GetDashboardStatsAsync(string? employeeId) //Hamza's method for getting stats
        {
            FilterDefinition<Ticket> filter;

            
            if (!string.IsNullOrEmpty(employeeId))
            {
                filter = Builders<Ticket>.Filter.Eq(t => t.CreatedBy.EmployeeId, employeeId);
            }
            else
            {
                
                filter = Builders<Ticket>.Filter.Empty;
            }

            
            var tickets = await _tickets.Find(filter).ToListAsync();

        
            int total = tickets.Count;
            int open = tickets.Count(t => t.Status == TicketStatus.open);
            int resolved = tickets.Count(t => t.Status == TicketStatus.resolved);
            int closed = tickets.Count(t => t.Status == TicketStatus.closed);

            return (total, open, resolved, closed);
        }

        public async Task<List<Ticket>> SearchTicketsAsync(string searchText, bool useAnd) //hamzas method for searching tickets(funtionality)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return await _tickets.Find(_ => true)
                                     .SortByDescending(t => t.CreatedAt)
                                     .ToListAsync();

            var words = searchText.Split(" ");

            var filters = words.Select(word =>
                Builders<Ticket>.Filter.Or(
                    Builders<Ticket>.Filter.Regex(t => t.Title, word),
                    Builders<Ticket>.Filter.Regex(t => t.Description, word)
                )
            );

            var finalFilter = useAnd
                ? Builders<Ticket>.Filter.And(filters)  
                : Builders<Ticket>.Filter.Or(filters); 

            return await _tickets.Find(finalFilter)
                                 .SortByDescending(t => t.CreatedAt) 
                                 .ToListAsync();
        }
    }
}

using System.Net.Sockets;
using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
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

        public async Task<List<Ticket>> GetAllTicketsAsync()
        {
            return await _tickets
                .Find(s => true)
                .SortByDescending(e => e.Status)  // open first
                .ThenBy(e => e.Priority)          // higher priority first
                .ToListAsync();
        }

        public async Task<List<Ticket>> GetTicketsByEmployeeIdAsync(EmployeeDetails employee)
        {
            return await _tickets
                .Find(t => t.CreatedBy.EmployeeId == employee.EmployeeId && t.Status == TicketStatus.open)
                .SortByDescending(e => e.CreatedAt)
                .ThenBy(e => e.Status)
                .ToListAsync();
        }

        public async Task UpdateTicketAsync(Ticket ticket)
        {
            var update = Builders<Ticket>.Update
    .Set(t => t.Status, ticket.Status)
    .Set(t => t.Title, ticket.Title)
    .Set(t => t.Description, ticket.Description);

            await _tickets.UpdateOneAsync(t => t.TicketId == ticket.TicketId, update);
        }

        public async Task<Ticket> GetByIdAsync(string id)
        {
            return await _tickets.Find(s => s.TicketId == id).FirstOrDefaultAsync();
        }

        public TicketViewModel FillTicketInfo(Ticket ticket)
        {
            return new TicketViewModel
            {
                Ticket = ticket,
                // Enums converted to select options
                StatusOptions = Enum.GetValues(typeof(TicketStatus))
            .Cast<TicketStatus>()
            .Select(s => new SelectListItem { Text = s.ToString(), Value = s.ToString() }),

                TypeOfIncidentOptions = Enum.GetValues(typeof(TypeOfIncident))
            .Cast<TypeOfIncident>()
            .Select(i => new SelectListItem { Text = i.ToString(), Value = i.ToString() }),

                PriorityOptions = Enum.GetValues(typeof(Priority))
            .Cast<Priority>()
            .Select(p => new SelectListItem { Text = p.ToString(), Value = p.ToString() })
            };
        }

        public async Task CreateTicketAsync(Ticket ticket)
        {
            await _tickets.InsertOneAsync(ticket);
        }

        public async Task<bool> CloseAsync(Ticket ticket)
        {
            var filter = Builders<Ticket>.Filter.Eq(e => e.TicketId, ticket.TicketId);
            var update = Builders<Ticket>.Update.Set(e => e.Status, TicketStatus.closed);

            var result = await _tickets.UpdateOneAsync(filter, update);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<(int total, int resolved, int transferred)> GetEmployeeStatsAsync(string firstName, string lastName)
        {

            var tickets = await _tickets.Find(_ => true).ToListAsync();


            var handledTickets = tickets
                .Where(t => t.ResolutionSteps.Last().PresentHandler != null &&
                    (t.ResolutionSteps.Last().PresentHandler.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase) ||
                     t.ResolutionSteps.Last().PresentHandler.LastName.Equals(lastName, StringComparison.OrdinalIgnoreCase)))
                .ToList();


            int total = handledTickets.Count;
            int resolved = handledTickets.Count(t => t.Status == TicketStatus.resolved || t.Status == TicketStatus.closed);


            int transferred = tickets
                .SelectMany(t => t.ResolutionSteps ?? new())
                .Count(s => s.PresentHandler != null &&
                    (s.PresentHandler.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase) ||
                     s.PresentHandler.LastName.Equals(lastName, StringComparison.OrdinalIgnoreCase)) &&
                    s.Action.Equals("Transferred", StringComparison.OrdinalIgnoreCase));


            return (total, resolved, transferred);
        }

        public async Task<List<Ticket>> SearchTicketsAsync(string searchText, bool useAnd)
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
                ? Builders<Ticket>.Filter.And(filters)  // ALL words must match
                : Builders<Ticket>.Filter.Or(filters);  // ANY word can match

            return await _tickets.Find(finalFilter)
                                 .SortByDescending(t => t.CreatedAt) // newest first
                                 .ToListAsync();
        }

<<<<<<< HEAD
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
                newDeadline = now;
            }


            return new EscalateViewModel(ticket, newPriority, newDeadline);
=======
        public async Task AddResolutionStep(string ticketId, EmployeeDetails details)
        {
            ResolutionStep step = new ResolutionStep(details, string.Empty);
            var filter = Builders<Ticket>.Filter.Eq(t => t.TicketId, ticketId);
            var update = Builders<Ticket>.Update.Push(t => t.ResolutionSteps, step);

            var result = await _tickets.UpdateOneAsync(filter, update);

            if (!result.IsAcknowledged)
            {
                throw new InvalidOperationException("Write not acknowledged by MongoDB.");
            }
            else { Console.WriteLine("Update successful"); }
>>>>>>> 97600f9e0183fb39a435bd6339b12c09ca18e65a
        }

    }
}

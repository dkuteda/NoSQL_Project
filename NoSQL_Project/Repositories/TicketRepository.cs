using MongoDB.Driver;
using NoSQL_Project.Models;

namespace NoSQL_Project.Repositories
{
    public class TicketRepository
    {
        private readonly IMongoCollection<Ticket> _tickets;

        public TicketRepository(IMongoDatabase db)
        {
            _tickets = db.GetCollection<Ticket>("tickets");
        }

        public async Task<List<Ticket>> GellAsync()
        {
            return await _tickets
                .Find(s => true)
                .SortByDescending(e => e.Status)  // open first
                .ThenBy(e => e.Priority)          // higher priority first
                .ToListAsync();
        }
    }
}

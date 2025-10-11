using Microsoft.AspNetCore.Mvc;
using NoSQL_Project.Services;
using NoSQL_Project.Services.Interfaces;

namespace NoSQL_Project.Controllers
{
    public class TicketController : Controller
    {
        private readonly ITicketServices _ticketService;

        public TicketController(ITicketServices ticketService) => _ticketService = ticketService;

        public IActionResult Index()
        {
            return View();
        }
    }
}

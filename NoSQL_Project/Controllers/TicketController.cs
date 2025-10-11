using Microsoft.AspNetCore.Mvc;
using NoSQL_Project.Models;
using NoSQL_Project.Services;
using NoSQL_Project.Services.Interfaces;
using NoSQL_Project.ViewModels;

namespace NoSQL_Project.Controllers
{
    public class TicketController : Controller
    {
        private readonly ITicketServices _ticketService;

        public TicketController(ITicketServices ticketService) => _ticketService = ticketService;

        public async Task<IActionResult> Index()
        {
            List<Ticket> tickets = await _ticketService.GellAsync();
            TicketViewModel ticketViewModel = new TicketViewModel
            {
                TicketList = tickets
            };

            return View(ticketViewModel);
        }
    }
}

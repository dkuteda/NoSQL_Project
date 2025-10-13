using Microsoft.AspNetCore.Mvc;
using NoSQL_Project.Models;
using NoSQL_Project.Services.Interfaces;
using NoSQL_Project.ViewModels;

namespace NoSQL_Project.Controllers
{
    [Route("TicketDashboard")]
    public class TicketsController : Controller
    {
        private readonly ITicketService _ticketService;

        public TicketsController(ITicketService ticketService) => _ticketService = ticketService;

        public async Task<IActionResult> Index()
        {
            List<TicketViewModel> tickets = await _ticketService.GetAllTicketsAsync();
            TicketDashboardViewModel ticketDashboardViewModel = new TicketDashboardViewModel
            {
                TicketList = tickets
            };

            return View("TicketDashboard", ticketDashboardViewModel);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NoSQL_Project.Enums;
using NoSQL_Project.Models;
using NoSQL_Project.Services;
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

        [HttpGet]
        public IActionResult UpdateTicket()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTicket(Ticket ticket)
        {
            try
            {
                await _ticketService.UpdateTicketAsync(ticket);
                TempData["SuccessMessage"] = "Ticket has been updated successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Exception occurred: {ex.Message}";
                return View();
            }
        }
    }
}

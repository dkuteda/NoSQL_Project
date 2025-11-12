using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NoSQL_Project.Enums;
using NoSQL_Project.Models;
using NoSQL_Project.Services;
using NoSQL_Project.Services.Interfaces;
using NoSQL_Project.ViewModels;

namespace NoSQL_Project.Controllers
{
    [Route("[controller]/[action]")]
    public class TicketsController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly IEmployeeService _employeeService;

        public TicketsController(ITicketService ticketService, IEmployeeService employeeService)
        {
            _ticketService = ticketService;
            _employeeService =employeeService;
        }

        public async Task<IActionResult> Index()
        {
            Employee employee = GetEmployeeFromSession();
            List<Ticket> tickets = await _ticketService.GetTicketsByEmployeeIdAsync(employee);
            TicketViewModel model = await FillViewModel(tickets);
            return View("MyTickets", model);
        }

        [HttpGet("TicketDetails")]
        public async Task<IActionResult> TicketDetails(string id)
        {
            EmployeeDetails presentHandler;
            bool isAssignee = false;
            var employee = GetEmployeeFromSession();
            List<Ticket> ticketList = await _ticketService.GetAllTicketsAsync();
            TicketViewModel model = new TicketViewModel()
            {
                TicketList = ticketList,
                PotentialTransferees = new List<Employee>()
            };
            Ticket ticket = model.Ticket = await _ticketService.GetByIdAsync(id);
            //TO DO: do something in case ticket is null
            if (ticket.ResolutionSteps.Count != 0)
            {
                presentHandler = ticket.ResolutionSteps.Last().PresentHandler;
                isAssignee = presentHandler != null && presentHandler.EmployeeId == employee.EmployeeId;
            }
            ViewData["LoggedInEmployee"] = employee;
            ViewData["IsAssignee"] = isAssignee;
            return View(model);
        }

        [HttpGet("TicketDashboard")]
        public async Task<IActionResult> TicketDashboard()
        {
            List<Ticket> tickets = await _ticketService.GetAllTicketsAsync();
            var model = new TicketViewModel()
            {
                TicketList = tickets
            };
            return View();
        }

        [HttpGet("UpdateTicket")]
        public IActionResult UpdateTicket(string id)
        {
            var ticket = _ticketService.GetByIdAsync(id).Result; // Synchronously wait for the result
            if (ticket == null)
            {
                return NotFound();
            }
            var ViewModel = _ticketService.FillTicketInfo(ticket);

            return View( ViewModel);
        }

        [HttpPost("UpdateTicket")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTicket(TicketViewModel ticketViewModel)
        {
            try
            {
                await _ticketService.UpdateTicketAsync(ticketViewModel.Ticket);
                TempData["SuccessMessage"] = "Ticket has been updated successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Exception occurred: {ex.Message}";
                return View( ticketViewModel);
            }
        }

        [HttpPost]
        public IActionResult AssignMyselfToTicket(Ticket ticket, EmployeeDetails details)
        {
            try 
            {
                _ticketService.AssignMyselfToTicket(ticket, details);
                return RedirectToAction("TicketDetails", new { id = ticket.TicketId });
            }
            catch (Exception ex) 
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("TicketDetails", new { id = ticket.TicketId });
            }
        }
        [HttpGet] //TO DO: Figure out a try catch here
        public async Task<IActionResult> OnSearch(string nameQuery, string ticketId)
        {
            List<Employee> potentialTransferees = await _employeeService.AutocompleteSearchEmployees(nameQuery);
            TicketViewModel model = new TicketViewModel
            {
                Ticket = await _ticketService.GetByIdAsync(ticketId),
                PotentialTransferees = potentialTransferees
            };
            ViewData["LoggedInEmployee"] = GetEmployeeFromSession();
            return View("TicketDetails", model);
        }

        [HttpPost]
        public IActionResult TransferTicket(string ticketId, EmployeeDetails details)
        {
            try
            {
                _ticketService.AddResolutionStep(ticketId, details);
                return RedirectToAction("TicketDetails", new { id = ticketId });
            }
            catch(Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("TicketDetails", new { id = ticketId });
            }
        }

        [HttpGet("AddTicket")]
        public IActionResult AddTicket()
        {
            ViewData["EmployeeDetails"] = GetEmployeeFromSession();
            var viewModel = new TicketViewModel
            {
                Ticket = new() { TicketId = Guid.NewGuid().ToString() }
            };
            return View(viewModel);
        }

        [HttpPost("AddTicket")]
        public async Task<IActionResult> AddTicket(TicketViewModel model)
        {
            try
            {
                model.Ticket.ResolutionSteps = model.Ticket.ResolutionSteps ?? new List<ResolutionStep>();
                await _ticketService.CreateTicketAsync(model.Ticket);

                TempData["SuccessMessage"] = "Ticket has been created successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                ViewBag.ErrorMessage = $"{ex}";
                return View("AddTicket", model);
            }
        }

        [HttpGet("CloseTicket")]
        public IActionResult CloseTicket(string id)
        {
            var ticket = _ticketService.GetByIdAsync(id).Result; // Synchronously wait for the result
            if (ticket == null)
            {
                return NotFound();
            }
            var ViewModel = _ticketService.FillTicketInfo(ticket);

            return View(ViewModel);
        }

        [HttpPost("CloseTicket")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseTicket(Ticket ticket)
        {
            try
            {
                bool isClosed = await _ticketService.CloseAsync(ticket);
                if (isClosed)
                {
                    TempData["SuccessMessage"] = "Ticket has been closed successfully";
                }
                else
                {
                    TempData["ErrorMessage"] = "Ticket not found or already closed";
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Exception occurred: {ex.Message}";
                var viewModel = new TicketViewModel
                {
                    Ticket = ticket,
                };
                return View("Index",viewModel);
            }
        }

        [HttpGet("Dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            // ✅ Get Employee object from session
            var employee = HttpContext.Session.GetObject<Employee>("LoggedInUser");

            // If session empty → go login
            if (employee == null)
                return RedirectToAction("Login", "Employees");

            // get all tickets or only user's tickets depending on role
            List<Ticket> tickets;
            if (employee.UserRole == UserRole.employee)
            {
                tickets = await _ticketService.GetTicketsByEmployeeIdAsync(employee);
            }
            else
            {
                tickets = await _ticketService.GetAllTicketsAsync();
            }

            int total = tickets.Count;
            int open = tickets.Count(t => t.Status == TicketStatus.open);
            int resolved = tickets.Count(t => t.Status == TicketStatus.resolved);
            int closed = tickets.Count(t => t.Status == TicketStatus.closed);

            var model = new DashboardViewModel
            {
                TotalTickets = total,
                OpenPercent = total > 0 ? (open * 100) / total : 0,
                ResolvedPercent = total > 0 ? (resolved * 100) / total : 0,
                ClosedPercent = total > 0 ? (closed * 100) / total : 0,
                TicketList = tickets.Take(5).ToList()
            };

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Search(string text, bool and = false)
        {
            var results = await _ticketService.SearchTicketsAsync(text, and);

            var model = new TicketViewModel
            {
                TicketList = results
            };

            return View("MyTickets", model);
        }

        private async Task<TicketViewModel> FillViewModel(List<Ticket>tickets)
        {
            ViewData["LoggedInEmployee"] = GetEmployeeFromSession();
            if (tickets.Count > 0 || tickets != null)
            {
                tickets = await _ticketService.GetAllTicketsAsync(); 
            }
            else tickets = new List<Ticket>();

            TicketViewModel model = new TicketViewModel()
            {
                TicketList = tickets,
                PotentialTransferees = new List<Employee>()
            };          
            return model;
        }

        private Employee GetEmployeeFromSession()
        {
            return HttpContext.Session.GetObject<Employee>("LoggedInUser");
        }

    }

}


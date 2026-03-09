using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using otrs_backend.Requests;
using otrs_backend.Services;
using System.Security.Claims;

namespace otrs_backend.Controllers
{
    [Route("api/ticket")]
    [ApiController]
    [Authorize]
    public class CreateTicketController : ControllerBase
    {
        private readonly TicketService _ticketService;

        public CreateTicketController(TicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] CreateTicketRequest request)
        {
            
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int currentUserId))
            {
                return Unauthorized("Nie można zidentyfikować użytkownika.");
            }

            try
            {
                var ticket = await _ticketService.CreateTicketAsync(request, currentUserId);

                return CreatedAtAction(nameof(GetTicketById), new { id = ticket.Id }, ticket);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicketById(int id)
        {
            //POBRANIE ID UŻYTKOWNIKA - żeby sprawdzić czy ma dostęp do tego zgłoszenia
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int currentUserId))
            {
                return Unauthorized("Nie można zidentyfikować użytkownika.");
            }
            return Ok(new { id = id, message = "Endpoint w budowie" });
        }
    }
}
